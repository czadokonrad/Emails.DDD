using Emails.DAL.Repositories;
using Emails.Domain.Entities;
using Emails.Reader.MailReaders.MailKit;
using FunctionalExtensionsLibrary.Exceptions;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Emails.Reader
{
    class Program
    { 
        static void Main(string[] args)
        {
            try
            {

                Result<EmailBox> emailBox = EmailBox.Create("konrad521@vp.pl", "imap.poczta.onet.pl", "Chedozycto3!!", 993, true, 6);

                emailBox.Value.GetType().BaseType.GetProperty("Id").SetValue(emailBox.Value, 2, null);

                ImapClient imapClient = new ImapClient();
                ProtocolLogger logger = new ProtocolLogger("smtp.log");
                CancellationTokenSource cts = new CancellationTokenSource();
                Console.WriteLine("Press q to cancel");


                Console.CancelKeyPress += (s, e) =>
                {
                    cts.Cancel();
                };

                using (IMapReader imapReader = new IMapReader(imapClient, logger,
                    new IMapFolderRepository(new DAL.EmailsContext()),
                    new EmailsRepository(new DAL.EmailsContext())))
                {
                    Task<IMailStore> connectTask = imapReader.ConnectAsync(emailBox.Value, cts.Token);

                    try
                    {
                        Task.WaitAll(new[] { connectTask }, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    catch (AggregateException)
                    {
                    }

                    Task readTask = imapReader.ReadAllAsync(connectTask.Result, emailBox.Value, cts.Token,
                        new Progress<double>((val) => Console.WriteLine($"{val} parsed")));


                    try
                    {
                        Task.WaitAll(new[] { readTask }, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    catch (AggregateException ex)
                    {
                    }
                    catch(Exception ex)
                    {

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            Console.ReadLine();

            Console.ReadLine();
        }
    }
}
