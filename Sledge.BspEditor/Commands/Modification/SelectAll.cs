﻿using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;

namespace Sledge.BspEditor.Commands.Modification
{
    [Export(typeof(ICommand))]
    [CommandID("BspEditor:Edit:SelectAll")]
    [DefaultHotkey("Ctrl+A")]
    [MenuItem("Edit", "", "Selection", "B")]
    public class SelectAll : BaseCommand
    {
        public override string Name => "Select All";
        public override string Details => "Select all objects";
        protected override Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var op = new Select(document.Map.Root.FindAll());
            return MapDocumentOperation.Perform(document, op);
        }
    }
}