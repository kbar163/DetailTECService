using Spire.Email;
using Spire.Email.Smtp;
using Spire.Email.IMap;
using HtmlAgilityPack;
using DetailTECService.Models;

namespace DetailTECService.Coms
{

    //Esta clase hace uso de las librerias HtmlAgilityPack (Para modificar el archivo HTML que se envia por correo a los clientes, personalizando con sus datos)
    //y la libreria Spire.Email para hacer el envio del correo usando protocolo smtp
    public static class EmailSender
    {
        public static void SendCreationEmail(Customer customer)
        {
            
            MailAddress sender = "detailtec.noreply@gmail.com";
            MailAddress recipient = customer.correo_cliente;
            MailMessage message = new MailMessage(sender,recipient);
            var password = customer.password_cliente;

            HtmlDocument emailDoc = new HtmlDocument();
            emailDoc.Load(@"Coms/email.html");
            var passwordSpace = emailDoc.GetElementbyId("password");
            passwordSpace.InnerHtml = customer.password_cliente;

            message.Subject = customer.nombre + ", Bienvenido a DetailTEC!";
            message.BodyHtml = emailDoc.DocumentNode.OuterHtml;
            message.Date = DateTime.Now;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.ConnectionProtocols = ConnectionProtocols.Ssl;
            smtp.Username = sender.Address;
            smtp.Password = "drpumtwzeggwojal";
            smtp.Port = 587;
            smtp.SendOne(message);


            
        }
    }
}