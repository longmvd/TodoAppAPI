﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Model
{
    public class Group: IBaseModel, ICreationInfo
    {
        private string _createdBy;
        private DateTime _createdDate;

        [Key]
        public int GroupID { get; set; }

        [Column]
        public string GroupName { get; set; }

        [Column]
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }

        [Column]
        public string CreatedBy { get => _createdBy; set => _createdBy = value; }

        public string GetTableName()
        {
            return "`group`";
        }

        public void SetPrimaryKey(dynamic key)
        {
            if(key is int)
            {
                GroupID = (int)key;
            }
            else
            {
                GroupID = 0;
            }
        }
    }
}
