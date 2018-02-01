using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tester_server.Connection;
using tester_server.Connection.Authentification;
using tester_server.Tester;

namespace tester_server
{
    class RequestHandler
    {
        private LoginManager login_manager;
        private string key;
        public RequestHandler()
        {
            this.login_manager = new LoginManager();
        }
        /// <summary>
        /// Sparsuje string na objekt triedy Request. dalsia akcia sa odvija od typu requestu
        /// </summary>
        /// <param name="message"></param>
        /// <returns> odpoved k danej poziadavke alebo null ak nie je poziadavka validna. Poziadavka bude zahodena</returns>
        public Response ProcessRequest(string message, string key)
        {
            Request parsed_request = Request.ConvertToRequest(message);
            Response resp = null;

            //neplatna ziadost
            if (parsed_request == null) return null;

            switch (parsed_request.type) {
                case SERVICE_TYPE.DISCONNECT:
                    resp = new Response(SERVICE_TYPE.DISCONNECT, "", RESULT_CODE.SUCCESS);
                    //odhlasenie
                    login_manager.LogOut(parsed_request.user.user_name);
                    break;

                case SERVICE_TYPE.EVAL:
                    //ak je uzivatel validny
                    if (CheckAuthentification(parsed_request.user))
                    {
                        //dummy vyhodnotenie
                    }
                    else
                    {

                    }
                    break;

                case SERVICE_TYPE.GET:
                    int id = 0;
                    //dummy vratenie id
                    TestTemplate test = TestManager.Instance().GetTest(id);
                    // test nie je dostupny
                    if(test == null)
                    {
                        resp = new Response(SERVICE_TYPE.GET, "", RESULT_CODE.UNAVAILABLE);
                    }
                    else
                    {
                        string data = test.ConvertTestToString();
                        if (data == null)
                        {
                            Console.Error.WriteLine("Error: data test");
                            data = "";
                        }
                        resp = new Response(SERVICE_TYPE.GET, data, RESULT_CODE.SUCCESS);
                    }
                  
                    break;

                case SERVICE_TYPE.LOGIN:
                    //spracovanie prijatych udajov
                    Account user = parsed_request.user;
                    RESULT_CODE code;
                    
                    //prihlasenie bolo uspesne
                    if (login_manager.LogIn(user.user_name, user.password_hash))
                    {
                        //namapuj
                        login_manager.MapUser(key, user.user_name);
                        //nastav vysledok
                        code = RESULT_CODE.SUCCESS;
                    }
                    else
                    {
                        code = RESULT_CODE.PERM_DENIED;
                    }
                    resp = new Response(SERVICE_TYPE.LOGIN, "", code);
                    break;

                case SERVICE_TYPE.TESTS_LIST:
                    //autentifikacia nepresla
                    if (!CheckAuthentification(parsed_request.user))
                    {
                        resp = new Response(SERVICE_TYPE.TESTS_LIST, "", RESULT_CODE.PERM_DENIED);
                    }
                    else
                    {
                        string tests_list = TestManager.Instance().GetTestsNames();
                        resp = new Response(SERVICE_TYPE.TESTS_LIST, tests_list, RESULT_CODE.SUCCESS);
                    }
                    break;
                default:
                    break; 
            }
            return resp;
        }

        public void RemoveMappedUser(string ip)
        {
            login_manager.RemoveMapedUser(ip);
        }

        private bool CheckAuthentification(Account tested)
        {
            return login_manager.IsLoggedIn(tested.user_name);
        }
    }
}
