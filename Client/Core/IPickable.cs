using System;
using System.Collections.Generic;
namespace Uniars.Client.Core
{
    interface IPickable<T>
    {
        void EnablePicker(Action<T> result);

        void EnablePicker(Action<List<T>> result);

        bool IsPickerEnabled();
    }
}
