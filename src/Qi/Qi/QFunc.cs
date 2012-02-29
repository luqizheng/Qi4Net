namespace Qi
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t1"></param>
    public delegate void VoidFunc<in T>(T t1);

    /// <summary>
    /// 
    /// </summary>
    public delegate void VoidFunc();

    public delegate void VoidFunc<in TT1, in TT2>(TT1 t1, TT2 t2);
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    public delegate void VoidFunc<in T1, in T2, in T3>(T1 t1, T2 t2, T3 t3);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    /// <param name="t4"></param>
    public delegate void VoidFunc<in T1, in T2, in T3, in T4>(T1 t1, T2 t2, T3 t3, T4 t4);
}