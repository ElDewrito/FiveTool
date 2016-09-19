using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;

namespace FiveTool.Scripting.Proxies
{
    /// <summary>
    /// Proxy class for lists.
    /// This is significantly faster than MoonSharp's default list-to-table conversion.
    /// </summary>
    [MoonSharpUserData]
    internal class ListProxy<T>
    {
        private readonly IList<T> _list;

        [MoonSharpHidden]
        public ListProxy(IList<T> list)
        {
            _list = list;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => _list.IsReadOnly;

        public T this[int index]
        {
            // Lua uses 1-based indices, so we need to use index - 1
            get { return _list[index - 1]; }
            set { _list[index - 1] = value; }
        }

        public void Add(T item) => _list.Add(item);

        public void Clear() => _list.Clear();

        public bool Contains(T item) => _list.Contains(item);

        public int IndexOf(T item) => _list.IndexOf(item);

        public void Insert(int index, T item) => _list.Insert(index, item);

        public bool Remove(T item) => _list.Remove(item);

        public void RemoveAt(int index) => _list.RemoveAt(index);

        public override string ToString() => $"{typeof(T).Name}[{Count}]";

        [MoonSharpUserDataMetamethod("__concat")]
        public static ListProxy<T> Concat(ListProxy<T> list1, ListProxy<T> list2)
        {
            var concatenated = new List<T>(list1.Count + list2.Count);
            concatenated.AddRange(list1._list);
            concatenated.AddRange(list2._list);
            return new ListProxy<T>(concatenated);
        }

        [MoonSharpUserDataMetamethod("__ipairs")]
        public static DynValue Ipairs(ListProxy<T> list)
        {
            // __ipairs must return a (callback, state, index) tuple.
            // The state needs to be a Lua object, but we can hack around this
            // by capturing the list in the callback and returning a nil state.
            var callback = new CallbackFunction((context, args) =>
            {
                var index = args.AsType(1, "ListProxyIpairsCallback", DataType.Number);
                var nextIndex = (int)index.Number + 1;
                if (nextIndex > list.Count)
                    return DynValue.Nil;
                var nextIndexValue = DynValue.NewNumber(nextIndex);
                var itemValue = DynValue.FromObject(context.GetScript(), list[nextIndex]);
                return DynValue.NewTuple(nextIndexValue, itemValue);
            });
            return DynValue.NewTuple(DynValue.NewCallback(callback), DynValue.Nil, DynValue.NewNumber(0));
        }
    }
}
