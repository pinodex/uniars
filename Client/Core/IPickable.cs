using System;
namespace Uniars.Client.Core
{
    interface IPickable<T>
    {
        void EnablePicker(Action<T> result);

        bool IsPickerEnabled();
    }
}
