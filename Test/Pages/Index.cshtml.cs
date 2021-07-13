using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.AspNetCore.Http;
using TestS3cr3txApp.Models;

namespace TestS3cr3txApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        
        public TestS3cr3txApp.Models.S3cr3tx S3Cr3Tx;
        public TestS3cr3txApp.Models.S3cr3txDbContext _s3cr3tx;
        public string _output;
        public IndexModel(TestS3cr3txApp.Models.S3cr3txDbContext s3Cr3tx)
        {
            _s3cr3tx = s3Cr3tx;
        }
        //public TestS3cr3txApp.S3cr3tx S3Cr3Tx;
        
        public void OnGet([Bind("Email", "AuthCode", "Token", "EoD", "Input","Output")] TestS3cr3txApp.Models.S3cr3tx _S3Cr3Tx)
        {
            S3Cr3Tx = _S3Cr3Tx;
        }
        public string Message { get; set; } = "";


        public void OnPostView()
        {
            HttpRequest Request = HttpContext.Request;
            if (Request.Form.TryGetValue("S3Cr3Tx.Email", out Microsoft.Extensions.Primitives.StringValues vEmail))
            {
                if (Request.Form.TryGetValue("S3Cr3Tx.AuthCode", out Microsoft.Extensions.Primitives.StringValues vCode))
                {
                    if (Request.Form.TryGetValue("S3Cr3Tx.Token", out Microsoft.Extensions.Primitives.StringValues vToken))
                    {
                        if (Request.Form.TryGetValue("S3Cr3Tx.EoD", out Microsoft.Extensions.Primitives.StringValues vEoD))
                        {
                            if (Request.Form.TryGetValue("S3Cr3Tx.Input", out Microsoft.Extensions.Primitives.StringValues vInput))
                            {
                                WebClient wc = new WebClient();
                                //wc.Credentials.GetCredential();
                                wc.BaseAddress = @"https://s3cr3tx.com/api/Values";
                                WebHeaderCollection webHeader = new WebHeaderCollection();
                                webHeader.Add(@"Email:" + vEmail[0]);
                                webHeader.Add(@"AuthCode:" + vCode[0]);
                                webHeader.Add(@"APIToken:" + vToken[0]);
                                webHeader.Add(@"Input:" + vInput[0]);
                                webHeader.Add(@"EorD:" + vEoD[0]);
                                webHeader.Add(@"Def:" + @"z");
                                wc.Headers = webHeader;
                                string result = wc.DownloadString(@"https://s3cr3tx.com/api/Values");
                               Message = result;
                                
                            }
                        }
                    }

                }
             }

            

        }
    }
}
