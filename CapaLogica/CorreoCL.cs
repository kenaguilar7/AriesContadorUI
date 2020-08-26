using CapaDatos.Daos;
using CapaEntidad.Entidades.IUsers;
using CapaEntidad.Enumeradores;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CapaLogica
{
    public class CorreoCL
    {
        CorreoDao correoDao = new CorreoDao();
        public DataTable Get()
        {

            return correoDao.Get();
        }
        public Boolean Insert(IUserTemporal IUserTemporal)
        {
            return correoDao.Insert(IUserTemporal);
        }

        public IEnumerable<IUserTemporal> SendMail(IEnumerable<IUserTemporal> IUsersTemporales)
        {
            var rejecteds = new List<IUserTemporal>();
            foreach (var user in IUsersTemporales)
            {

                if (!Push(user))
                {
                    rejecteds.Add(user);
                    user.EstadoEnvio = EstadoEnvio.Fallido;
                }
                else
                {
                    user.EstadoEnvio = EstadoEnvio.Correcto;
                }

                Insert(user);
            }
            return rejecteds;
        }
        public Boolean Push(IUserTemporal IUser)
        {
            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("ariescontador@gmail.com", "116390867A.ab"),
                    // UseDefaultCredentials = true,
                    // DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = true
                };
                MailMessage mail = new MailMessage();
                //Setting From , To and CC
                mail.From = new MailAddress("ariescontador@gmail.com", "Sistemas Aries");
                mail.To.Add(new MailAddress(IUser.CorreoElectronico));
                mail.CC.Add(new MailAddress(IUser.CCopy));
                mail.Subject = IUser.Asunto;
                mail.IsBodyHtml = true;
                mail.Body = $"<html><nav><h1>{IUser.Titulo}</h1><span>{IUser.Cuerpo}</span><h6></h6></nav></html>";
                client.Send(mail);
                //Console.WriteLine("Sent");
                //Console.ReadLine();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
