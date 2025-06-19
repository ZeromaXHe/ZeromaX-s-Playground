namespace TO.Domains.Interfaces.Commons.WithLength

open System.Collections.Generic

/// Copyright (C) 2025 Zhu Xiaohe(aka ZeromaXHe)
/// Author: Zhu XH (ZeromaXHe)
/// Date: 2025-06-19 14:08:19
[<Interface>]
type IWithLength<'T> =
    inherit IEnumerable<'T>
    abstract Length: int
    abstract Item: int -> 'T // 只读的索引属性的接口抽象形式，就是直接这样写！

type WithLengthEnumerator<'T>(withLength: 'T IWithLength) =
    let mutable currentIdx = -1

    interface 'T IEnumerator with
        override this.Current = withLength[currentIdx]
        override this.Current: obj = withLength[currentIdx] // IEnumerator 非泛型版本的 get 属性

        override this.MoveNext() =
            currentIdx <- currentIdx + 1
            currentIdx < withLength.Length

        override this.Reset() = currentIdx <- -1
        override this.Dispose() = ()
