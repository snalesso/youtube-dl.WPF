using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    public abstract class ValueObject<T> : IEquatable<T>
        where T : ValueObject<T>
    {
        public ValueObject()
        {
            this._hashCodeCache = new Lazy<int>(() => HashCodeHelper.CombineHashCodes(this.GetValueIngredients()), LazyThreadSafetyMode.PublicationOnly);
        }

        /// <summary>
        /// Returns the <see cref="object"/>s used by <see cref="GetHashCode()"/> to compose the <see cref="T"/>'s value as a <see cref="ValueObject{T}"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        // TODO: benchmark signature with IReadOnlyList<object>, which forces to return an array, but safer
        protected abstract IEnumerable<object> GetValueIngredients();

        private readonly Lazy<int> _hashCodeCache;
        /// <summary>
        /// Provides the hashcode of the the <see cref="T"/>'s value as a <see cref="ValueObject{T}"/>.
        /// </summary>
        /// <returns></returns>
        public sealed override int GetHashCode()
        {
            return this._hashCodeCache.Value;
        }

        ///// <summary>
        ///// Compares this <see cref="T"/> with another <see cref="T"/> != <see langword="null"/>.
        ///// </summary>
        ///// <param name="other"></param>
        ///// <returns></returns>
        //protected abstract bool EqualsCore(T other);

        /// <summary>
        /// Compares two <see cref="T"/>s.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(T other)
        {
            return
                !(other is null)
                && this.GetType() == other.GetType() // TODO: does this work if I pass an inheriting class instance??
                && this.GetValueIngredients().SequenceEqual(other.GetValueIngredients());
        }

        /// <summary>
        /// Compares the current <see cref="T"/> with another <see cref="object"/> checking if, as a whole, they represent the same value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as T); // TODO: add type checking: test derived type casting
        }

        /// <summary>
        /// Checks if the two <see cref="T"/>s are equal by reference or value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        {
            if (left is null ^ right is null)
                return false;

            return left is null || left.Equals(right);
        }

        /// <summary>
        /// Checks if the two <see cref="T"/>s are not equal by reference or value.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        {
            return !(left == right);
        }
    }
}
