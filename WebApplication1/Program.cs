using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// Ho tro chain method, tuc la mot chuoi co the ... lien tuc duoc. Builder giup chung ta co the . ra nhieu thu ben trong.
/// Cu . xong roi append.
/// 
/// </summary>

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var builder = new WebHostBuilder()
                .UseKestrel() //de chi ra server la kestrel
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory()) //chi ra thu muc hien tai cua ung dung, la thu muc goc cua UD(ConfigurationSample)
                .ConfigureAppConfiguration((hostingContext, config) =>//configure app (nhan vao 2 cai: hostingcontext va config, cung dung body, dua vao ham nac danh(vo danh). no se nhan vao mot cai la WebBuilderContext va 1 cai la ConfigurationBuilder
                {
                    var env = hostingContext.HostingEnvironment; //lay ra duoc moi truong hien tai la ten moi truong, tu day chung ta co the cau hinh la UD nay se nhan file config nao
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) //file config cua no la appsettings.json, tham so thu 2 la optional:true => co tim thay hay khong thi no cung khong bao loi, neu optional:false thi co nghia la bat buoc; reloadOnChange:true nghia la se tu dong nhan cau hinh moi trong TH co thay doi, neu khong thi thoi, khong can phai restart lai UD
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);  //co file mac dinh la file appsettings.json, chung ta cung co cac file cho cac moi truong khac nhau, vd: o mtruong development chung ta muon no nhan cac cau hinh o development

                    //chung ta co the cau hinh them user secret. Neu mtruong development we se dung den secret. Secret nay se load trong assembly hien tai cua chung ta, chinh la assemly cua ung dung, chinh la ConfigurationSample.dll nay. No load assembly hien tai, sau do no load secret khi no debug
                    if (env.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                        {
                            config.AddUserSecrets(appAssembly, optional: true);
                        }
                    }

                    //lay tham so he thong(environment variable)
                    config.AddEnvironmentVariables();

                    //neu trong TH CommandLine argument khac null thi no cung lay tu commandline
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }

                })

                .ConfigureLogging((hostingContext, logging) => //config logg(tuong tu cung nhan vao 1 hostingContext va 1 cai logging)
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection(key: "Logging")); //trong appsettings.json, level cua no mac dinh la warning thi no se write ra. chung ta write ra 2 thang do la cua so console va cua so debug
                    logging.AddConsole();
                    logging.AddDebug();

                })

                //chung ta muon chay UD nay tren IIS de tich hop voi Kestrel se dung cau hinh nay
                .UseIISIntegration()

                //ngoai ra we cau hinh ... chi cho Development thoi
                .UseDefaultServiceProvider((context, options)=> 
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                ;

            return builder;
        }
            
    }
}
