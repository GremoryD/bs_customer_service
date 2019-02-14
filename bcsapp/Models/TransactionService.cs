using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace bcsapp.Models
{
    public static class TransactionService
    {
        private static TimeSpan timeout = TimeSpan.FromSeconds(10);

        public static List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public static void AddUser(Transaction userAddTransaction)
        {
            Transactions.Add(userAddTransaction);

            EventHandler<String> onDoneMessageRecieved = null;
            EventHandler<string> onErrorMessageRecieved = null;

            onDoneMessageRecieved = new EventHandler<String>((_, userClass) =>
            {
                userAddTransaction.Complete();
                Transactions.Remove(userAddTransaction);
                
                WebSocketController.Instance.UpdateUserAdd -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateUserAdd_Err -= onErrorMessageRecieved;
            });
            onErrorMessageRecieved = new EventHandler<string>((_, mess) =>
            {
                userAddTransaction.Revert();
                Transactions.Remove(userAddTransaction);

                WebSocketController.Instance.UpdateUserAdd -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateUserAdd_Err -= onErrorMessageRecieved;
            }); 
            WebSocketController.Instance.UpdateUserAdd += onDoneMessageRecieved;
            WebSocketController.Instance.LoginFailed += onErrorMessageRecieved; 

            var timer = new Timer(timeout.TotalMilliseconds)  { AutoReset = false  };
            timer.Elapsed += (_, __) => { onErrorMessageRecieved(_, "Timeout."); };
            timer.Start(); 

            WebSocketController.Instance.OutputQueueAddObject(userAddTransaction.Data);
        }

        public static void EditUser(Transaction userEditTransaction)
        {
            Transactions.Add(userEditTransaction);

            EventHandler<String> onDoneMessageRecieved = null;
            EventHandler<string> onErrorMessageRecieved = null;

            onDoneMessageRecieved = new EventHandler<String>((_, userClass) =>
            {
                userEditTransaction.Complete();
                Transactions.Remove(userEditTransaction);

                WebSocketController.Instance.UpdateUserEdit -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateUserEdit_Err -= onErrorMessageRecieved;
            });
            onErrorMessageRecieved = new EventHandler<string>((_, mess) =>
            {
                userEditTransaction.Revert();
                Transactions.Remove(userEditTransaction);

                WebSocketController.Instance.UpdateUserEdit -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateUserEdit_Err -= onErrorMessageRecieved;
            });
            WebSocketController.Instance.UpdateUserEdit += onDoneMessageRecieved;
            WebSocketController.Instance.UpdateUserEdit_Err += onErrorMessageRecieved;

            var timer = new Timer(timeout.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (_, __) => { onErrorMessageRecieved(_, "Timeout."); };
            timer.Start();

            WebSocketController.Instance.OutputQueueAddObject(userEditTransaction.Data);
        }


        public static void AddJob(Transaction jobAddTransaction)
        {
            Transactions.Add(jobAddTransaction);

            EventHandler<String> onDoneMessageRecieved = null;
            EventHandler<string> onErrorMessageRecieved = null;

            onDoneMessageRecieved = new EventHandler<String>((_, userClass) =>
            {
                jobAddTransaction.Complete();
                Transactions.Remove(jobAddTransaction);

                WebSocketController.Instance.UpdateJobAdd -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateJobAdd_Err -= onErrorMessageRecieved;
            });
            onErrorMessageRecieved = new EventHandler<string>((_, mess) =>
            {
                jobAddTransaction.Revert();
                Transactions.Remove(jobAddTransaction);

                WebSocketController.Instance.UpdateJobAdd -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateJobAdd_Err -= onErrorMessageRecieved;
            });
            WebSocketController.Instance.UpdateJobAdd += onDoneMessageRecieved;
            WebSocketController.Instance.UpdateJobAdd_Err += onErrorMessageRecieved;

            var timer = new Timer(timeout.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (_, __) => { onErrorMessageRecieved(_, "Timeout."); };
            timer.Start();

            WebSocketController.Instance.OutputQueueAddObject(jobAddTransaction.Data);
        }

        public static void EditJob(Transaction jobEditTransaction)
        {
            Transactions.Add(jobEditTransaction);

            EventHandler<String> onDoneMessageRecieved = null;
            EventHandler<string> onErrorMessageRecieved = null;

            onDoneMessageRecieved = new EventHandler<String>((_, userClass) =>
            {
                jobEditTransaction.Complete();
                Transactions.Remove(jobEditTransaction);

                WebSocketController.Instance.UpdateJobEdit -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateJobEdit_Err -= onErrorMessageRecieved;
            });
            onErrorMessageRecieved = new EventHandler<string>((_, mess) =>
            {
                jobEditTransaction.Revert();
                Transactions.Remove(jobEditTransaction);

                WebSocketController.Instance.UpdateJobEdit -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateJobEdit_Err -= onErrorMessageRecieved;
            });
            WebSocketController.Instance.UpdateJobEdit += onDoneMessageRecieved;
            WebSocketController.Instance.UpdateJobEdit_Err += onErrorMessageRecieved;

            var timer = new Timer(timeout.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (_, __) => { onErrorMessageRecieved(_, "Timeout."); };
            timer.Start();

            WebSocketController.Instance.OutputQueueAddObject(jobEditTransaction.Data);
        }

        public static void AddRole(Transaction roleAddTransaction)
        {
            Transactions.Add(roleAddTransaction);

            EventHandler<String> onDoneMessageRecieved = null;
            EventHandler<string> onErrorMessageRecieved = null;

            onDoneMessageRecieved = new EventHandler<String>((_, userClass) =>
            {
                roleAddTransaction.Complete();
                Transactions.Remove(roleAddTransaction);

                WebSocketController.Instance.UpdateRoleAdd -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateRoleAdd_Err -= onErrorMessageRecieved;
            });
            onErrorMessageRecieved = new EventHandler<string>((_, mess) =>
            {
                roleAddTransaction.Revert();
                Transactions.Remove(roleAddTransaction);

                WebSocketController.Instance.UpdateRoleAdd -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateRoleAdd_Err -= onErrorMessageRecieved;
            });
            WebSocketController.Instance.UpdateRoleAdd += onDoneMessageRecieved;
            WebSocketController.Instance.UpdateRoleAdd_Err += onErrorMessageRecieved;

            var timer = new Timer(timeout.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (_, __) => { onErrorMessageRecieved(_, "Timeout."); };
            timer.Start();

            WebSocketController.Instance.OutputQueueAddObject(roleAddTransaction.Data);
        }

        public static void EditRole(Transaction roleEditTransaction)
        {
            Transactions.Add(roleEditTransaction);

            EventHandler<String> onDoneMessageRecieved = null;
            EventHandler<string> onErrorMessageRecieved = null;

            onDoneMessageRecieved = new EventHandler<String>((_, userClass) =>
            {
                roleEditTransaction.Complete();
                Transactions.Remove(roleEditTransaction);

                WebSocketController.Instance.UpdateRoleEdit -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateRoleEdit_Err -= onErrorMessageRecieved;
            });
            onErrorMessageRecieved = new EventHandler<string>((_, mess) =>
            {
                roleEditTransaction.Revert();
                Transactions.Remove(roleEditTransaction);

                WebSocketController.Instance.UpdateRoleEdit -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateRoleEdit_Err -= onErrorMessageRecieved;
            });
            WebSocketController.Instance.UpdateRoleEdit += onDoneMessageRecieved;
            WebSocketController.Instance.UpdateRoleEdit_Err += onErrorMessageRecieved;

            var timer = new Timer(timeout.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (_, __) => { onErrorMessageRecieved(_, "Timeout."); };
            timer.Start();

            WebSocketController.Instance.OutputQueueAddObject(roleEditTransaction.Data);
        }

        public static void AddRoleToUser(Transaction roleAddUserTransaction)
        {
            Transactions.Add(roleAddUserTransaction);

            EventHandler<String> onDoneMessageRecieved = null;
            EventHandler<string> onErrorMessageRecieved = null;

            onDoneMessageRecieved = new EventHandler<String>((_, userClass) =>
            {
                roleAddUserTransaction.Complete();
                Transactions.Remove(roleAddUserTransaction);

                WebSocketController.Instance.UpdateUserRoleAdd -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateUserRoleAdd_Err -= onErrorMessageRecieved;
            });
            onErrorMessageRecieved = new EventHandler<string>((_, mess) =>
            {
                roleAddUserTransaction.Revert();
                Transactions.Remove(roleAddUserTransaction);

                WebSocketController.Instance.UpdateUserRoleAdd -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateUserRoleAdd_Err -= onErrorMessageRecieved;
            });
            WebSocketController.Instance.UpdateUserRoleAdd += onDoneMessageRecieved;
            WebSocketController.Instance.UpdateUserRoleAdd_Err += onErrorMessageRecieved;

            var timer = new Timer(timeout.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (_, __) => { onErrorMessageRecieved(_, "Timeout."); };
            timer.Start();

            WebSocketController.Instance.OutputQueueAddObject(roleAddUserTransaction.Data);
        }


        public static void RemoveRoleToUser(Transaction roleRemoveUserTransaction)
        {
            Transactions.Add(roleRemoveUserTransaction);

            EventHandler<String> onDoneMessageRecieved = null;
            EventHandler<string> onErrorMessageRecieved = null;

            onDoneMessageRecieved = new EventHandler<String>((_, userClass) =>
            {
                roleRemoveUserTransaction.Complete();
                Transactions.Remove(roleRemoveUserTransaction);

                WebSocketController.Instance.UpdateUserRoleRemove -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateUserRoleRemove_Err -= onErrorMessageRecieved;
            });
            onErrorMessageRecieved = new EventHandler<string>((_, mess) =>
            {
                roleRemoveUserTransaction.Revert();
                Transactions.Remove(roleRemoveUserTransaction);

                WebSocketController.Instance.UpdateUserRoleRemove -= onDoneMessageRecieved;
                WebSocketController.Instance.UpdateUserRoleRemove_Err -= onErrorMessageRecieved;
            });
            WebSocketController.Instance.UpdateUserRoleRemove += onDoneMessageRecieved;
            WebSocketController.Instance.UpdateUserRoleRemove_Err += onErrorMessageRecieved;

            var timer = new Timer(timeout.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (_, __) => { onErrorMessageRecieved(_, "Timeout."); };
            timer.Start();

            WebSocketController.Instance.OutputQueueAddObject(roleRemoveUserTransaction.Data);
        }


         

    }

    public class Transaction
    {
        public ServerLib.JTypes.Client.RequestBaseClass Data;
        public Action Complete;
        public Action Revert;

        public Transaction(ServerLib.JTypes.Client.RequestBaseClass message, Action complete, Action revert)
        {
            Data = message;
            Complete = complete;
            Revert = revert;
        }
    }
}
