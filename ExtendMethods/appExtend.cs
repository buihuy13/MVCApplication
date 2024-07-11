namespace MVCTest.ExtendMethods
{
    public static class appExtend
    {
        public static void AddStatusCodePage(this IApplicationBuilder app)
        {
            app.UseStatusCodePages(appError =>
            {
                appError.Run(async context =>
                {
                    var response = context.Response;
                    var statusCode = response.StatusCode;

                    var content = @$"<html>
                                  <head>
                                     <meta charset = 'UTF-8' />
                                     <title>Lỗi {statusCode}</title>
                                  </head>
                                  <body>
                                       <p style='color:red; font-size: 30px'> Có lỗi xảy ra {statusCode} </p>
                                  </body>
                                 </html>";

                    await response.WriteAsync(content);
                });
            });
        }
    }
}
