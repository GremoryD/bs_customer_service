﻿using ServerLib.JTypes.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcsapp.ViewModels
{
    public class UserViewModel : IViewModel
    {
        public UserViewModel()
        {   
            WebSocketController.Instance.OutputQueueAddObject(new UsersClass());

        }

    }
}
