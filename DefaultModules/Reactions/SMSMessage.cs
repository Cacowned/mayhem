using System;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Windows;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace DefaultModules.Reactions
{
	[Serializable]
	public class SmsMessage : ReactionBase, IWpf
	{
		//list of US provider gateways: http://hacknmod.com/hack/email-to-text-messages-for-att-verizon-t-mobile-sprint-virgin-more/

		protected string to;
		protected string from;
		protected string password; // ick.
		protected string subject;
		protected string mailServer;
		protected string msg;

		public SmsMessage()
			: base("SMS Message", "Sends a text message when triggered.") {

			to = "";
			subject = "Mayhem";
			msg = "Mayhem just triggered!";

			Setup();
		}

		public void Setup() {
			hasConfig = true;
			SetConfigString();
		}

		public void SetConfigString() {
			ConfigString = String.Format("To: {0}\nSubject: {1}\nMessage: {2}", to, subject, msg);
		}

		public override void Perform() {
			MailMessage message = new MailMessage(from, to, subject, msg);
			SmtpClient mySmtpClient = new SmtpClient(mailServer);
			mySmtpClient.Credentials = new NetworkCredential(from, password);

			mySmtpClient.Send(message);
		}

		public void WpfConfig() {
			var window = new SmsMessageConfig(to, subject, msg, mailServer, from, password);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			window.ShowDialog();

			if (window.DialogResult == true) {

				to = window.to;
				subject = window.subject;
				msg = window.msg;
				mailServer = window.mailServer;
				from = window.from;
				password = window.password;

				SetConfigString();
			}
		}

		#region Serialization
		public SmsMessage(SerializationInfo info, StreamingContext context)
			: base(info, context) {

			to = info.GetString("To");
			from = info.GetString("From");
			password = info.GetString("Password");
			subject = info.GetString("Subject");
			mailServer = info.GetString("MailServer");
			msg = info.GetString("Message");

			Setup();
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

			info.AddValue("To", to);
			info.AddValue("From", from);
			info.AddValue("Password", password);
			info.AddValue("Subject", subject);
			info.AddValue("MailServer", mailServer);
			info.AddValue("Message", msg);
		}
		#endregion
	}
}
