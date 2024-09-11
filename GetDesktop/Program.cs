using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WebApplication2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        app.UseHttpsRedirection();

        app.MapGet("/Get/Desktop", (HttpContext httpContext) =>
        {
            var mimeType = "image/png";
            return Results.File(DPIUtil.GetDesktopScreen(), contentType: mimeType);
        });

        app.Run();
    }
}
