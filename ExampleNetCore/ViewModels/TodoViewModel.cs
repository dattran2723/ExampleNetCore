﻿using ExampleNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleNetCore.ViewModels
{
    public class TodoViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }
        public UserViewModel UserAssign { get; set; }
    }
}
