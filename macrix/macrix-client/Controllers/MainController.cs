using System;
using System.Collections.Generic;
using System.Threading;
using macrix_client.Data;
using macrix_client.Controllers;

using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace macrix_client.Controllers
{
    class MainController
    {

        private readonly IActionsService _actionsService;

        public MainController(IActionsService actionsService)
        {
            _actionsService = actionsService;
        }

        List<User> Users = new List<User>();
        public void Start()
        {
            Thread tid1 = new Thread(new ThreadStart(Thread1));
            tid1.Start();
        }

        public void Thread1()
        {
            _actionsService.RenderHeader();
            _actionsService.RenderDashboard();
            
            Console.ReadLine();
        }
    }
}
