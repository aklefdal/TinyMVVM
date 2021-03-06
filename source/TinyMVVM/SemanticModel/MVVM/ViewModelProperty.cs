﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMVVM.Specifications.SemanticModel;

namespace TinyMVVM.SemanticModel.MVVM
{
    public class ViewModelProperty
    {
    	private ViewModelPropertyIsPrimitiveType isPrimitiveType = new ViewModelPropertyIsPrimitiveType();

        public string Name { get; protected set; }
        public string Type { get; protected set; }
        public bool IsObservable { get; protected set; }
        public List<string> Attributes { get; protected set; }

    	public bool IsPrimitiveType
    	{
    		get
    		{
    			return isPrimitiveType.IsSatisfiedBy(this);
    		}
    	}

        public ViewModelProperty(string name, 
            string type,
            bool isObservable)
        {
            if (name == null || type == null)
                throw new ArgumentNullException();

            Name = name;
            Type = type;
            IsObservable = isObservable;

            Attributes = new List<string>();
        }


    }
}
