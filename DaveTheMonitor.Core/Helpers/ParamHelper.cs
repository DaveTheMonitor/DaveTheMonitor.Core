namespace DaveTheMonitor.Core.Helpers
{
    /// <summary>
    /// Contains methods for verifying that <see cref="object"/> parameters are of the specified types.
    /// </summary>
    public static class ParamHelper
    {
        public static bool VerifyArgs<T0>(object[] args, out T0 arg0)
        {
            if (args[0] is T0 t0)
            {
                arg0 = t0;
                return true;
            }
            arg0 = default;
            return true;
        }

        public static bool VerifyArgs<T0, T1>(object[] args,
            out T0 arg0,
            out T1 arg1)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1)
            {
                arg0 = t0;
                arg1 = t1;
                return true;
            }
            arg0 = default;
            arg1 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5, T6>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5,
            out T6 arg6)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5 &&
                args[6] is T6 t6)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                arg6 = t6;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            arg6 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5, T6, T7>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5,
            out T6 arg6,
            out T7 arg7)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5 &&
                args[6] is T6 t6 &&
                args[7] is T7 t7)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                arg6 = t6;
                arg7 = t7;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            arg6 = default;
            arg7 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5, T6, T7, T8>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5,
            out T6 arg6,
            out T7 arg7,
            out T8 arg8)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5 &&
                args[6] is T6 t6 &&
                args[7] is T7 t7 &&
                args[8] is T8 t8)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                arg6 = t6;
                arg7 = t7;
                arg8 = t8;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            arg6 = default;
            arg7 = default;
            arg8 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5,
            out T6 arg6,
            out T7 arg7,
            out T8 arg8,
            out T9 arg9)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5 &&
                args[6] is T6 t6 &&
                args[7] is T7 t7 &&
                args[8] is T8 t8 &&
                args[9] is T9 t9)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                arg6 = t6;
                arg7 = t7;
                arg8 = t8;
                arg9 = t9;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            arg6 = default;
            arg7 = default;
            arg8 = default;
            arg9 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5,
            out T6 arg6,
            out T7 arg7,
            out T8 arg8,
            out T9 arg9,
            out T10 arg10)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5 &&
                args[6] is T6 t6 &&
                args[7] is T7 t7 &&
                args[8] is T8 t8 &&
                args[9] is T9 t9 &&
                args[10] is T10 t10)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                arg6 = t6;
                arg7 = t7;
                arg8 = t8;
                arg9 = t9;
                arg10 = t10;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            arg6 = default;
            arg7 = default;
            arg8 = default;
            arg9 = default;
            arg10 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5,
            out T6 arg6,
            out T7 arg7,
            out T8 arg8,
            out T9 arg9,
            out T10 arg10,
            out T11 arg11)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5 &&
                args[6] is T6 t6 &&
                args[7] is T7 t7 &&
                args[8] is T8 t8 &&
                args[9] is T9 t9 &&
                args[10] is T10 t10 &&
                args[11] is T11 t11)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                arg6 = t6;
                arg7 = t7;
                arg8 = t8;
                arg9 = t9;
                arg10 = t10;
                arg11 = t11;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            arg6 = default;
            arg7 = default;
            arg8 = default;
            arg9 = default;
            arg10 = default;
            arg11 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5,
            out T6 arg6,
            out T7 arg7,
            out T8 arg8,
            out T9 arg9,
            out T10 arg10,
            out T11 arg11,
            out T12 arg12)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5 &&
                args[6] is T6 t6 &&
                args[7] is T7 t7 &&
                args[8] is T8 t8 &&
                args[9] is T9 t9 &&
                args[10] is T10 t10 &&
                args[11] is T11 t11 &&
                args[12] is T12 t12)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                arg6 = t6;
                arg7 = t7;
                arg8 = t8;
                arg9 = t9;
                arg10 = t10;
                arg11 = t11;
                arg12 = t12;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            arg6 = default;
            arg7 = default;
            arg8 = default;
            arg9 = default;
            arg10 = default;
            arg11 = default;
            arg12 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5,
            out T6 arg6,
            out T7 arg7,
            out T8 arg8,
            out T9 arg9,
            out T10 arg10,
            out T11 arg11,
            out T12 arg12,
            out T13 arg13)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5 &&
                args[6] is T6 t6 &&
                args[7] is T7 t7 &&
                args[8] is T8 t8 &&
                args[9] is T9 t9 &&
                args[10] is T10 t10 &&
                args[11] is T11 t11 &&
                args[12] is T12 t12 &&
                args[13] is T13 t13)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                arg6 = t6;
                arg7 = t7;
                arg8 = t8;
                arg9 = t9;
                arg10 = t10;
                arg11 = t11;
                arg12 = t12;
                arg13 = t13;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            arg6 = default;
            arg7 = default;
            arg8 = default;
            arg9 = default;
            arg10 = default;
            arg11 = default;
            arg12 = default;
            arg13 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5,
            out T6 arg6,
            out T7 arg7,
            out T8 arg8,
            out T9 arg9,
            out T10 arg10,
            out T11 arg11,
            out T12 arg12,
            out T13 arg13,
            out T14 arg14)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5 &&
                args[6] is T6 t6 &&
                args[7] is T7 t7 &&
                args[8] is T8 t8 &&
                args[9] is T9 t9 &&
                args[10] is T10 t10 &&
                args[11] is T11 t11 &&
                args[12] is T12 t12 &&
                args[13] is T13 t13 &&
                args[14] is T14 t14)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                arg6 = t6;
                arg7 = t7;
                arg8 = t8;
                arg9 = t9;
                arg10 = t10;
                arg11 = t11;
                arg12 = t12;
                arg13 = t13;
                arg14 = t14;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            arg6 = default;
            arg7 = default;
            arg8 = default;
            arg9 = default;
            arg10 = default;
            arg11 = default;
            arg12 = default;
            arg13 = default;
            arg14 = default;
            return false;
        }

        public static bool VerifyArgs<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(object[] args,
            out T0 arg0,
            out T1 arg1,
            out T2 arg2,
            out T3 arg3,
            out T4 arg4,
            out T5 arg5,
            out T6 arg6,
            out T7 arg7,
            out T8 arg8,
            out T9 arg9,
            out T10 arg10,
            out T11 arg11,
            out T12 arg12,
            out T13 arg13,
            out T14 arg14,
            out T15 arg15)
        {
            if (args[0] is T0 t0 &&
                args[1] is T1 t1 &&
                args[2] is T2 t2 &&
                args[3] is T3 t3 &&
                args[4] is T4 t4 &&
                args[5] is T5 t5 &&
                args[6] is T6 t6 &&
                args[7] is T7 t7 &&
                args[8] is T8 t8 &&
                args[9] is T9 t9 &&
                args[10] is T10 t10 &&
                args[11] is T11 t11 &&
                args[12] is T12 t12 &&
                args[13] is T13 t13 &&
                args[14] is T14 t14 &&
                args[15] is T15 t15)
            {
                arg0 = t0;
                arg1 = t1;
                arg2 = t2;
                arg3 = t3;
                arg4 = t4;
                arg5 = t5;
                arg6 = t6;
                arg7 = t7;
                arg8 = t8;
                arg9 = t9;
                arg10 = t10;
                arg11 = t11;
                arg12 = t12;
                arg13 = t13;
                arg14 = t14;
                arg15 = t15;
                return true;
            }
            arg0 = default;
            arg1 = default;
            arg2 = default;
            arg3 = default;
            arg4 = default;
            arg5 = default;
            arg6 = default;
            arg7 = default;
            arg8 = default;
            arg9 = default;
            arg10 = default;
            arg11 = default;
            arg12 = default;
            arg13 = default;
            arg14 = default;
            arg15 = default;
            return false;
        }
    }
}
