using System.Collections;

namespace Qi.Web.JsonContainers
{
    internal class ListAccesser : Accesser
    {
        private readonly IList _target;

        public ListAccesser(IList target) : base(target)
        {
            _target = target;
        }

        public override int Count
        {
            get { return _target.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="startIndex"></param>
        public override void Set(IEnumerable objects, int startIndex)
        {
            while (startIndex > _target.Count)
            {
                _target.Add(null);
            }
            foreach (object o in objects)
            {
                _target[startIndex] = o;
                startIndex++;
            }
        }
    }
}