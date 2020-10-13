
using MetadataExtractor;
using System;
using System.IO;

namespace resource.preview
{
    public class VSPreview : cartridge.AnyPreview
    {
        protected override void _Execute(atom.Trace context, string url)
        {
            if (File.Exists(url))
            {
                var a_Context = ImageMetadataReader.ReadMetadata(url);
                {
                    var a_Size = GetProperty(NAME.PROPERTY.LIMIT_PREVIEW_SIZE);
                    {
                        a_Size = Math.Min(a_Size, __GetParam(a_Context, "Height") / CONSTANT.OUTPUT_PREVIEW_ITEM_HEIGHT);
                        a_Size = Math.Max(a_Size, CONSTANT.OUTPUT_PREVIEW_MIN_SIZE);
                    }
                    for (var i = 0; i < a_Size; i++)
                    {
                        context.
                            SetState(NAME.STATE.VIDEO).
                            Send(NAME.PATTERN.PREVIEW, 1, "");
                    }
                }
                {
                    context.
                        SetState(NAME.STATE.FOOTER).
                        Send(NAME.PATTERN.ELEMENT, 1, "[[Size]]: " + __GetParam(a_Context, "Width").ToString() + " x " + __GetParam(a_Context, "Height").ToString());
                }
                foreach (var a_Context1 in a_Context)
                {
                    {
                        context.
                            Send(NAME.PATTERN.FOLDER, 2, a_Context1.Name);
                    }
                    foreach (var a_Context2 in a_Context1.Tags)
                    {
                        context.
                            SetValue(a_Context2.Description).
                            Send(NAME.PATTERN.VARIABLE, 3, a_Context2.Name);
                    }
                    if (a_Context1.HasError == false)
                    {
                        continue;
                    }
                    foreach (var a_Context2 in a_Context1.Errors)
                    {
                        context.
                            SetState(NAME.STATE.ERROR).
                            Send(NAME.PATTERN.ELEMENT, 3, a_Context2);
                    }
                }
            }
            else
            {
                context.
                    SendError(1, "[[File not found]]");
            }
        }

        private static int __GetParam(System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory> context, string name)
        {
            foreach (var a_Context in context)
            {
                foreach (var a_Context1 in a_Context.Tags)
                {
                    var a_Result = 0.0;
                    if ((a_Context1.Name == name) && double.TryParse(a_Context1.Description, out a_Result))
                    {
                        if (a_Result > 0)
                        {
                            return (int)a_Result;
                        }
                    }
                }
            }
            return 0;
        }
    };
}
