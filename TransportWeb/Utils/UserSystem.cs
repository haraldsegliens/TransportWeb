using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Security.Cryptography;
using System.Web.SessionState;
using TransportWeb.Models;
using System.Text;
using System.Data.Entity.Validation;
using System.Web;

namespace TransportWeb.Utils
{
    public static class UserSystem
    {
        public static bool IsAuthorized(HttpSessionStateBase session,String access)
        {
            var user = IsAuthenticated(session);
            return user.Access == access;
        }

        public static SessionData_User IsAuthenticated(HttpSessionStateBase session)
        {
            if (session == null)
                return null;

            if (session["user"] == null)
                return null;
            var user = session["user"] as SessionData_User;
            if (!ValidateSessionKey(user))
                return null;
            return user;
        }

        public static bool Authenticate(HttpSessionStateBase session,String username,String password)
        {
            if (session == null)
                return false;
            TransportWeb_DataModelContainer db = new TransportWeb_DataModelContainer();
            var db_user = db.Users.FirstOrDefault(u => u.Username == username);
            if (db_user == null)
                return false;
            if (!SecurePasswordHasher.Verify(password, db_user.Password_Cache))
                return false;

            var session_user = new SessionData_User();
            session_user.Username = username;
            session_user.Access = db_user.Access;
            if(db_user.Session == null || db_user.Session.Length == 0)
            {
                var rnd = new Random();
                Byte[] b = new Byte[32];
                rnd.NextBytes(b);
                session_user.SessionKey = Encoding.ASCII.GetString(b);
                db_user.Session = session_user.SessionKey;
                db.SaveChanges();
            }
            else
            {
                session_user.SessionKey = db_user.Session;
            }
            session["user"] = session_user;
            return true;
        }

        public static void Unauthenticate(HttpSessionStateBase session)
        {
            var user = session["user"] as SessionData_User;
            TransportWeb_DataModelContainer db = new TransportWeb_DataModelContainer();
            var db_user = db.Users.FirstOrDefault(u => u.Username == user.Username);
            if (db_user == null)
                return;
            db_user.Session = "";
            db.SaveChanges();
        }

        public static bool Authorize(string username, string password, string access)
        {
            TransportWeb_DataModelContainer db = new TransportWeb_DataModelContainer();
            var db_user = db.Users.FirstOrDefault(u => u.Username == username);
            if (db_user != null)
                return false;
            var user = db.Users.Add(new User());
            user.Username = username;
            user.Access = access;
            user.Password_Salt = " ";
            user.Password_Cache = SecurePasswordHasher.Hash(password);
            try { 
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return true;
        }

        public static void Unauthorize(string username)
        {
            TransportWeb_DataModelContainer db = new TransportWeb_DataModelContainer();
            var db_user = db.Users.FirstOrDefault(u => u.Username == username);
            if (db_user == null)
                return;
            db.Users.Remove(db_user);
            db.SaveChanges();
        }

        private static bool ValidateSessionKey(SessionData_User user)
        {
            TransportWeb_DataModelContainer db = new TransportWeb_DataModelContainer();
            var sessionKey = db.Users.Where(u => u.Username == user.Username).Select(u => u.Session).FirstOrDefault();
            if (sessionKey == null || sessionKey.Length == 0)
                return false;
            return sessionKey == user.SessionKey;
        }
    }
}

public sealed class SecurePasswordHasher
{
    /// <summary>
    /// Size of salt
    /// </summary>
    private const int SaltSize = 16;

    /// <summary>
    /// Size of hash
    /// </summary>
    private const int HashSize = 20;

    /// <summary>
    /// Creates a hash from a password
    /// </summary>
    /// <param name="password">the password</param>
    /// <param name="iterations">number of iterations</param>
    /// <returns>the hash</returns>
    public static string Hash(string password, int iterations)
    {
        //create salt
        byte[] salt;
        new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);

        //create hash
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        var hash = pbkdf2.GetBytes(HashSize);

        //combine salt and hash
        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

        //convert to base64
        var base64Hash = Convert.ToBase64String(hashBytes);

        //format hash with extra information
        return string.Format("$MYHASH$V1${0}${1}", iterations, base64Hash);
    }
    /// <summary>
    /// Creates a hash from a password with 10000 iterations
    /// </summary>
    /// <param name="password">the password</param>
    /// <returns>the hash</returns>
    public static string Hash(string password)
    {
        return Hash(password, 10000);
    }

    /// <summary>
    /// Check if hash is supported
    /// </summary>
    /// <param name="hashString">the hash</param>
    /// <returns>is supported?</returns>
    public static bool IsHashSupported(string hashString)
    {
        return hashString.Contains("$MYHASH$V1$");
    }

    /// <summary>
    /// verify a password against a hash
    /// </summary>
    /// <param name="password">the password</param>
    /// <param name="hashedPassword">the hash</param>
    /// <returns>could be verified?</returns>
    public static bool Verify(string password, string hashedPassword)
    {
        //check hash
        if (!IsHashSupported(hashedPassword))
        {
            throw new NotSupportedException("The hashtype is not supported");
        }

        //extract iteration and Base64 string
        var splittedHashString = hashedPassword.Replace("$MYHASH$V1$", "").Split('$');
        var iterations = int.Parse(splittedHashString[0]);
        var base64Hash = splittedHashString[1];

        //get hashbytes
        var hashBytes = Convert.FromBase64String(base64Hash);

        //get salt
        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        //create hash with given salt
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
        byte[] hash = pbkdf2.GetBytes(HashSize);

        //get result
        for (var i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
            {
                return false;
            }
        }
        return true;
    }
}