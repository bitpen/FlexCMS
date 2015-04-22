using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bCommon.Security
{
    public class SQLUserPassAuth : IAuthenticate
    {
        private AccountContext _db;
        public SQLUserPassAuth()
        {
            _db = new AccountContext();
        }

        public bool ChangePassword(string userId, string email, string oldPassword, string newPassword)
        {
            if (Authenticate(userId, email, oldPassword))
            {
                var account = GetAccountModel(userId, email);
                account.PasswordHash = PasswordHash.CreateHash(newPassword);
                _db.Entry(account).State = EntityState.Modified;
                _db.SaveChanges();

                return true;
            }

            return false;
        }

        public bool CreateAccount(string userId, string password, string email)
        {
            try
            {
                if (String.IsNullOrEmpty(userId))
                {
                    throw new ArgumentNullException("userId", "A unique user id required to create a new account.");
                }

                if (String.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException("password", "A password is required to create a new account.");
                }

                if (String.IsNullOrEmpty(email))
                {
                    throw new ArgumentNullException("email", "A unique email required to create a new account.");
                }

                if (GetAccountModel(userId, null) != null)
                {
                    return false;
                }

                if (GetAccountModel(null, email) != null)
                {
                    return false;
                }

                var account = new Account();
                account.Id = Guid.NewGuid(); //placeholder for validation
                account.DateCreated = DateTime.Now;
                account.CreatedBy = "system";
                account.IsActive = true;
                account.UserId = userId;
                account.Email = email;
                account.PasswordHash = PasswordHash.CreateHash(password);


                _db.Accounts.Add(account);
                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DisableAccount(string userId, string email)
        {
            var account = GetAccountModel(userId, email);

            if (account == null)
            {
                return false;
            }

            account.IsActive = false;
            _db.Entry(account).State = EntityState.Modified;
            _db.SaveChanges();

            return true;
        }

        public object GetAccount(string userId, string email)
        {
            throw new NotImplementedException();
        }

        public string ResetPassword(string userId, string email)
        {
            throw new NotImplementedException();
        }

        public bool Authenticate(string userId, string email, string password)
        {
            var account = GetAccountModel(userId, email);
            if (account == null)
            {
                return false;
            }

            return PasswordHash.ValidatePassword(password, account.PasswordHash);
        }


        private Account GetAccountModel(string userId, string email)
        {
            if (!String.IsNullOrEmpty(userId) && String.IsNullOrEmpty(email))
            {
                return
                    _db.Accounts.FirstOrDefault(i => i.UserId.ToLower().Equals(userId.ToLower()));
            }
            else if (String.IsNullOrEmpty(userId) && !String.IsNullOrEmpty(email))
            {
                return _db.Accounts.FirstOrDefault(i => i.Email.Equals(email));
            }

            return null;
        }


        private class AccountContext : DbContext
        {
            public AccountContext() : base("AccountContext") { }

            public DbSet<Account> Accounts { get; set; }
        }

        [Table("Account")]
        private class Account
        {
            [Key]
            public Guid Id { get; set; }

            [RegularExpression(@"[0-9a-zA-Z_\-]{1,50}")]
            [MaxLength(50)]
            [Required]
            public String UserId { get; set; }

            [MaxLength(250)]
            [Required]
            public String Email { get; set; }

            [Required]
            public String PasswordHash { get; set; }

            [Required]
            public DateTime DateCreated { get; set; }

            [MaxLength(50)]
            [Required]
            public String CreatedBy { get; set; }

            public DateTime? DateModified { get; set; }

            [MaxLength(50)]
            public String ModifiedBy { get; set; }

            public Boolean IsActive { get; set; }
        }


    }
}
