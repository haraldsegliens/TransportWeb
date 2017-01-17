using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Security.Cryptography;
using System.Web.SessionState;
using TransportWeb.Models;
using System.Text;
using System.Data.Entity.Validation;

namespace TransportWeb.Utils
{
    public static class UserSystem
    {
        public static bool IsAuthorized(HttpSessionState session,String access)
        {
            if (session == null)
                return false;

            if (session["user"] == null)
                return false;
            var user = session["user"] as SessionData_User;
            if (!ValidateSessionKey(user))
                return false;
            return user.Access == access;
        }

        public static bool Authenticate(HttpSessionState session,String username,String password)
        {
            if (session == null)
                return false;
            TransportWeb_DataModelContainer db = new TransportWeb_DataModelContainer();
            var db_user = db.Users.FirstOrDefault(u => u.Username == username);
            if (db_user == null)
                return false;
            if (!VerifyPassword(password, Encoding.ASCII.GetBytes(db_user.Password_Salt), Encoding.ASCII.GetBytes(db_user.Password_Cache)))
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

        public static void Unauthenticate(HttpSessionState session)
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
            byte[] salt = GenerateSalt();

            var user = db.Users.Add(new User());
            user.Username = username;
            user.Access = access;
            user.Password_Salt = Encoding.ASCII.GetString(GenerateSalt());
            user.Password_Cache = Encoding.ASCII.GetString(ComputeHash(username, salt));
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
        
        // 24 = 192 bits
        private const int SaltByteSize = 24;
        private const int HashByteSize = 24;
        private const int HasingIterationsCount = 10101;

        private static byte[] ComputeHash(string password, byte[] salt, int iterations = HasingIterationsCount, int hashByteSize = HashByteSize)
        {
            Rfc2898DeriveBytes hashGenerator = new Rfc2898DeriveBytes(password, salt);
            hashGenerator.IterationCount = iterations;
            return hashGenerator.GetBytes(hashByteSize);
        }

        private static byte[] GenerateSalt(int saltByteSize = SaltByteSize)
        {
            RNGCryptoServiceProvider saltGenerator = new RNGCryptoServiceProvider();
            byte[] salt = new byte[saltByteSize];
            saltGenerator.GetBytes(salt);
            return salt;
        }

        private static bool VerifyPassword(String password, byte[] passwordSalt, byte[] passwordHash)
        {
            byte[] computedHash = ComputeHash(password, passwordSalt);
            return AreHashesEqual(computedHash, passwordHash);
        }

        //Length constant verification - prevents timing attack
        private static bool AreHashesEqual(byte[] firstHash, byte[] secondHash)
        {
            int minHashLenght = firstHash.Length <= secondHash.Length ? firstHash.Length : secondHash.Length;
            var xor = firstHash.Length ^ secondHash.Length;
            for (int i = 0; i < minHashLenght; i++)
                xor |= firstHash[i] ^ secondHash[i];
            return 0 == xor;
        }
    }
}