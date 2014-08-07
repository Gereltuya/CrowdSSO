CrowdSSO
========

Implementation of authentication, passwords, details for the Crowd Single Sign On service.

Provided:

Authentication
Change Password
Request Password Reset E-Mail
Retreive User Detail

Requires:

Newtonsoft.Json

Example:

Authenticates the user and then adds the users details to a cookie for retreival later.


                    CrowdSSO sso = new CrowdSSO("http://localhost/crowd/", "exampleAppName", "z2Ndj8RxMQik%Ruf^Hs0!WO7j#");
                    bool authorised = sso.Authenticate(model.username, model.password);

                    if (authorised)
                    {
                        UserDetail UserDetail = new UserDetail();

                        UserDetail = sso.UserDetail(model.username);

                        FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                              1,                                     // ticket version
                              model.username,                        // authenticated username
                              DateTime.Now,                          // issueDate
                              DateTime.Now.AddDays(30),              // expiryDate
                              true,                                  // true to persist across browser sessions
                              UserDetail.ToString(),                // serialise a UserDetail object
                              FormsAuthentication.FormsCookiePath    // the path for the cookie
                        );

                        // Encrypt the ticket using the machine key
                        string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                        // Add the cookie to the request to save it
                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
