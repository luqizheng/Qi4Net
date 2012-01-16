using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qi
{
    public delegate void VoidFunc<T>(T t1);

    public delegate void VoidFunc();

    public delegate void VoidFunc<T1, T2, T3>(T1 t1, T2 t2, T3 t3);
    
    public delegate void VoidFunc<T1, T2, T3,T4>(T1 t1, T2 t2, T3 t3,T4 t4);


}
