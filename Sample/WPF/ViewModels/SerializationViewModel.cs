﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using Codeplex.Reactive;
using Codeplex.Reactive.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WPF.ViewModels
{
    public class SerializationViewModel
    {
        public ReactiveCollection<long> Items { get; private set; }
        public ReactiveProperty<bool> IsChedked { get; private set; }
        public ReactiveProperty<int> SelectedIndex { get; private set; }
        [DataMember(Order = 3)]
        public ReactiveProperty<string> Text { get; private set; }
        [DataMember(Order = 2)]
        public ReactiveProperty<int> SliderPosition { get; private set; }
        public ReactiveCommand Serialize { get; private set; }
        public ReactiveCommand Deserialize { get; private set; }

        public SerializationViewModel()
        {
            // Observable sequence to ObservableCollection
            Items = Observable.Interval(TimeSpan.FromSeconds(1))
                .Take(30)
                .ToReactiveCollection();
            IsChedked = new ReactiveProperty<bool>();
            SelectedIndex = new ReactiveProperty<int>();
            Text = new ReactiveProperty<string>();
            SliderPosition = new ReactiveProperty<int>();

            var serializedString = new ReactiveProperty<string>(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);
            Serialize = serializedString.Select(x => x == null).ToReactiveCommand();
            Deserialize = serializedString.Select(x => x != null).ToReactiveCommand();

            // Click Serialize Button
            Serialize.Subscribe(_ =>
            {
                // Serialize ViewModel's all ReactiveProperty Values.
                // return value is string that Serialize by DataContractSerializer.
                serializedString.Value = SerializeHelper.PackReactivePropertyValue(this); // this = ViewModel
            });

            // Click Deserialize Button
            Deserialize.Subscribe(_ =>
            {
                // Deserialize to target ViewModel.
                // Deseirlization order is same as DataContract.
                // Can control DataMemberAttribute's Order Property.
                // more info see http://msdn.microsoft.com/en-us/library/ms729813.aspx
                SerializeHelper.UnpackReactivePropertyValue(this, serializedString.Value);

                serializedString.Value = null; // push to command canExecute
            });
        }
    }
}