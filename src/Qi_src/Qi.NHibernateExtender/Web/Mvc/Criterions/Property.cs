using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iesi.Collections;

namespace Qi.Web.Mvc.Criterions
{
    /// <summary>
    /// 把字符串转为object类型。
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public delegate object ConvertTo(string val);
    /// <summary>
    /// 查询接口
    /// </summary>
    public class Property
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ConvertTo StringToBoject { get; set; }
        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public Property Clone()
        {
            return new Property
            {
                StringToBoject = StringToBoject,
                Name = Name,
                Content = Content
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// 
        /// </summary>
        Asc,
        /// <summary>
        /// 
        /// </summary>
        Desc,
    }
    /// <summary>
    /// 
    /// </summary>
    public class PropertyData
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public SortOrder? Order { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class CriterionPostData
    {
        public PropertyData[] PropertyDatas { get; set; }
    }
}
