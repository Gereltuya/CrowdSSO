public class UserDetail
    {
        public int ID { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string VectusUsername { get; set; }
        public int ParabisEmployeeID { get; set; }
        public int SessionID { get; set; }

        // Serialize    
        public override string ToString()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string result = serializer.Serialize(this);
            return result;
        }

        // Deserialize
        public static UserDetail FromString(string text)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<UserDetail>(text);
        }

        public UserDetail FromCookie()
        {
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value);

            UserDetail userDetail = new UserDetail();

            userDetail = UserDetail.FromString(ticket.UserData);

            return userDetail;
        }

        public int SessionFromCookie()
        {
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value);

            UserDetail userDetail = new UserDetail();

            userDetail = UserDetail.FromString(ticket.UserData);

            return userDetail.SessionID;
        }
    }
