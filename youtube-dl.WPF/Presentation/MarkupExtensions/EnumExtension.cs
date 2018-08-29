using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace youtube_dl.WPF.Presentation.MarkupExtensions
{
    public partial class EnumExtension : MarkupExtension
    {
        private Type _enumType;

        public EnumExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            this.EnumType = enumType;
        }

        public Type EnumType
        {
            get { return this._enumType; }
            private set
            {
                if (this._enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                this._enumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(this.EnumType);
            var enumMembers = new List<EnumMember>();

            foreach (var enumValue in enumValues)
            {
                enumMembers.Add(new EnumMember(enumValue/*, this.GetDescription(enumValue)*/));
            }

            return enumMembers.AsReadOnly();
        }

        //private string GetDescription(object enumValue)
        //{
        //    var descriptionAttribute = this.EnumType
        //      .GetField(enumValue.ToString())
        //      .GetCustomAttributes(typeof(DescriptionAttribute), false)
        //      .FirstOrDefault() as DescriptionAttribute;

        //    return descriptionAttribute != null
        //      ? descriptionAttribute.Description
        //      : enumValue.ToString();
        //}
    }
}