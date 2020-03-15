using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace youtube_dl.WPF.Core
{
    public abstract class Entity<TIdentity> : /*Entity, IEntity,*/ IEquatable<Entity<TIdentity>>, INotifyPropertyChanged
        where TIdentity : IEquatable<TIdentity>
    {
        #region artificial id

        protected Entity(TIdentity id)
        {
            this.EnsureIsWellFormattedId(id);

            this.Id = id;
        }

        public TIdentity Id { get; }

        #endregion

        //#region Entity

        //protected override IEnumerable<object> GetIdentityIngredients()
        //{
        //    yield return this.Id;
        //}

        //#endregion

        #region Entity<TIdentity>

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Entity<TIdentity>);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public bool Equals(Entity<TIdentity> other)
        {
            if (other is null)
                return false;

            if (this.GetType() != other.GetType())
                return false;

            return this.Id.Equals(other.Id);
        }

        protected abstract void EnsureIsWellFormattedId(TIdentity id);

        #endregion

        #region operators

        public static bool operator ==(Entity<TIdentity> left, Entity<TIdentity> right)
        {
            if (left is null ^ right is null)
                return false;

            return (left is null) || left.Equals(right);
        }

        public static bool operator !=(Entity<TIdentity> left, Entity<TIdentity> right)
        {
            return !(left == right);
        }

        #endregion

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        public TProperty SetAndRaiseIfChanged<TProperty>(
            ref TProperty backingField,
            TProperty newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (EqualityComparer<TProperty>.Default.Equals(backingField, newValue))
                return newValue;

            backingField = newValue;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            return newValue;
        }

        #endregion
    }
}
