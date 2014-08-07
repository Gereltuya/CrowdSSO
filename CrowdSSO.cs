using KnowledgeBase.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

public class CrowdSSO
    {
        /// <summary>
        /// Instatiate the single sign on service
        /// </summary>
        /// <param name="crowdUrl">Url of the Crowd Server instance</param>
        /// <param name="appName">The app name in Crowd</param>
        /// <param name="appPassword">The password for the app in Crowd</param>
        public CrowdSSO(string crowdUrl, string appName, string appPassword)
        {
            crowdLocation = crowdUrl;
            applicationName = appName;
            applicationPassword = appPassword;
        }

        #region Global Variables

        private string applicationName = string.Empty;
        private string applicationPassword = string.Empty;
        private string crowdLocation = string.Empty;

        private const string displayName = "display-name";
        private const string email = "email";
        private const string firstName = "first-name";
        private const string lastName = "last-name";

        #endregion

        /// <summary>
        /// Authenticate against Crowd
        /// </summary>
        /// <param name="username">The username of the user that is set in Crowd</param>
        /// <param name="password">The password of the user that is set in Crowd</param>
        /// <returns>Returns TRUE if the user has provided correct credentials. Otherwise False.</returns>
        /// <remarks>Throws WebException if authentication fails</remarks>
        public HttpStatusCode Authenticate(string username, string password)
        {
            var request = (HttpWebRequest)WebRequest.Create(crowdLocation + "rest/usermanagement/1/authentication?username=" + username);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = "POST";
            request.Headers[HttpRequestHeader.Authorization] = string.Format("Basic " + Encode(applicationName, applicationPassword));

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                var json = JsonConvert.SerializeObject(
                    new
                    {
                        value = password
                    });

                writer.Write(json);

            }
            try
            {
                var result = (HttpWebResponse)request.GetResponse();
                return result.StatusCode;
            }
            catch (WebException)
            {
                //Log it, alert someone and then return 
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Change user's password in Crowd
        /// </summary>
        /// <param name="username">The username of the user that is set in Crowd</param>
        /// <param name="password">The new password for the user</param>
        /// <returns>Returns TRUE if the user has provided correct credentials. Otherwise False.</returns>
        /// <remarks>Throws WebException if authentication fails</remarks>
        public HttpStatusCode ChangePassword(string username, string password)
        {
            var request = (HttpWebRequest)WebRequest.Create(crowdLocation + "rest/usermanagement/1/user/password?username=" + username);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = "POST";
            request.Headers[HttpRequestHeader.Authorization] = string.Format("Basic " + Encode(applicationName, applicationPassword));

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                var json = JsonConvert.SerializeObject(
                    new
                    {
                        value = password
                    });

                writer.Write(json);

            }
            try
            {
                var result = (HttpWebResponse)request.GetResponse();
                return result.StatusCode;
            }
            catch (WebException)
            {
                //Log it, alert someone and then return 
                return HttpStatusCode.InternalServerError;
            }
        }

        /// <summary>
        /// Request an e-mail link to reset the user password
        /// </summary>
        /// <param name="username">The username of the user that is set in Crowd</param>
        /// <returns>Returns TRUE if the user exists and a pasword reset e-mail is sent. Otherwise False.</returns>
        /// <remarks>Throws WebException if authentication fails</remarks>
        public HttpStatusCode RequestPasswordResetemail(string username)
        {
            var request = (HttpWebRequest)WebRequest.Create(crowdLocation + "rest/usermanagement/1/user/mail/password?username=" + username);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = "POST";
            request.Headers[HttpRequestHeader.Authorization] = string.Format("Basic " + Encode(applicationName, applicationPassword));

            try
            {
                var result = (HttpWebResponse)request.GetResponse();
                return result.StatusCode;
            }
            catch (WebException)
            {
                //Log it, alert someone and then return 
                return HttpStatusCode.InternalServerError;
            }
        }


        /// <summary>
        /// Get user details from Crowd
        /// </summary>
        /// <param name="username">The username of the user that is set in Crowd</param>
        /// <returns>Returns UserDetail object</returns>
        /// <remarks>Throws WebException if authentication fails</remarks>
        public UserDetail UserDetail(string username)
        {

            UserDetail details = new UserDetail();

            var request = (HttpWebRequest)WebRequest.Create(crowdLocation + "rest/usermanagement/1/user?username=" + username);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = "GET";
            request.Headers[HttpRequestHeader.Authorization] = string.Format("Basic " + Encode(applicationName, applicationPassword));

            try
            {
                var result = (HttpWebResponse)request.GetResponse();
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    using (var reader = new StreamReader(result.GetResponseStream()))
                    {
                        var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.ReadToEnd());
                        details.FirstName = json[firstName].ToString();
                        details.LastName = json[lastName].ToString();
                        details.Username = json[displayName].ToString();
                        details.Email = json[email].ToString();
                    }
                    return details;
                }
            }
            catch (WebException)
            {
                //Log it, alert someone and then return the empty details
                return details;
            }
            return details;
        }


        // Generic code we will use
        #region Private Methods

        private static string Encode(string username, string password)
        {
            var auth = string.Join(":", username, password);
            return Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(auth));
        }

        #endregion
    }
