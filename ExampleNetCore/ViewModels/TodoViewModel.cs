using ExampleNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleNetCore.ViewModels
{
    public class TodoViewModel
    {
        public TodoItem TodoItem { get; set; }
        public User UserAssign { get; set; }
    }
}
