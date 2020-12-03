
using MetadataExtractor;
using System;
using System.IO;

namespace resource.preview
{
    internal class VSPreview : cartridge.AnyPreview
    {
        protected override void _Execute(atom.Trace context, string url)
        {
            if (File.Exists(url))
            {
                var a_Context = ImageMetadataReader.ReadMetadata(url);
                {
                    context.
                        SetState(NAME.STATE.HEADER).
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, 1, "[[Info]]");
                    {
                        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 2, "[[File Name]]", url);
                        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 2, "[[File Size]]", (new System.IO.FileInfo(url)).Length.ToString());
                        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 2, "[[Media Types]]", "Audio, Video");
                        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 2, "[[Raw Format]]", __GetString(a_Context, "Expected File Name Extension").ToUpper());
                        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 2, "[[Tag Types]]", __GetString(a_Context, "Detected File Type Name"));
                    }
                }
                {
                    var a_Size = GetProperty(NAME.PROPERTY.PREVIEW_MEDIA_SIZE);
                    {
                        a_Size = Math.Min(a_Size, __GetInteger(a_Context, "Height") / CONSTANT.OUTPUT_PREVIEW_ITEM_HEIGHT);
                        a_Size = Math.Max(a_Size, CONSTANT.OUTPUT_PREVIEW_MIN_SIZE);
                    }
                    for (var i = 0; i < a_Size; i++)
                    {
                        context.
                            Send(NAME.SOURCE.PREVIEW, NAME.TYPE.PREVIEW, 1, "");
                    }
                }
                {
                    context.
                        SetState(NAME.STATE.FOOTER).
                        Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, 1, "[[Size]]: " + __GetInteger(a_Context, "Width").ToString() + " x " + __GetInteger(a_Context, "Height").ToString());
                    {
                        context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.FOLDER, 2, "[[Codecs]]");
                        {
                            context.
                                SetComment("[[Audio]]").
                                SetCommentHint("[[Media Type]]").
                                Send(NAME.SOURCE.PREVIEW, NAME.TYPE.CLASS, 3, __GetString(a_Context, "Expected File Name Extension").ToUpper() + " Audio (" + __GetString(a_Context, "Major Brand") + ")");
                            {
                                context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.INFO, 4, "[[Header]]");
                                {
                                    context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 5, "[[Channels]]", __GetChannels(a_Context).ToString());
                                    context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 5, "[[Duration]]", __GetDuration(a_Context));
                                    context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 5, "[[Media Types]]", "[[Audio]]");
                                    context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 5, "[[Preferred Rate]]", __GetInteger(a_Context, "Preferred Rate").ToString());
                                    context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 5, "[[Preferred Volume]]", __GetInteger(a_Context, "Preferred Volume").ToString());
                                }
                            }
                        }
                        {
                            context.
                                SetComment("[[Video]]").
                                SetCommentHint("[[Media Type]]").
                                Send(NAME.SOURCE.PREVIEW, NAME.TYPE.CLASS, 3, __GetString(a_Context, "Expected File Name Extension").ToUpper() + " Video (" + __GetString(a_Context, "Major Brand") + ")");
                            {
                                context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.INFO, 4, "[[Header]]");
                                {
                                    context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 5, "[[Duration]]", __GetDuration(a_Context));
                                    context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 5, "[[Media Types]]", "[[Video]]");
                                }
                            }
                            {
                                context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.INFO, 4, "[[Size]]");
                                {
                                    context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 5, "[[Width]]", __GetInteger(a_Context, "Width").ToString());
                                    context.Send(NAME.SOURCE.PREVIEW, NAME.TYPE.VARIABLE, 5, "[[Height]]", __GetInteger(a_Context, "Height").ToString());
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                context.
                    SendError(1, "[[File not found]]");
            }
        }

        private static int __GetChannels(System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory> context)
        {
            var a_Result = 0;
            foreach (var a_Context in context)
            {
                if (a_Context.Name == "QuickTime Track Header")
                {
                    a_Result++;
                }
            }
            return a_Result;
        }

        private static string __GetDuration(System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory> context)
        {
            var a_Result = TimeSpan.FromMilliseconds(__GetInteger(context, "Duration"));
            {
                return
                    a_Result.Hours.ToString("D2") + ":" +
                    a_Result.Minutes.ToString("D2") + ":" +
                    a_Result.Seconds.ToString("D2") + "." +
                    a_Result.Milliseconds.ToString("D3");
            }
        }

        private static string __GetString(System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory> context, string name)
        {
            foreach (var a_Context in context)
            {
                foreach (var a_Context1 in a_Context.Tags)
                {
                    if ((a_Context1.Name == name) && (string.IsNullOrEmpty(a_Context1.Description) == false))
                    {
                        return a_Context1.Description?.Trim();
                    }
                }
            }
            return "";
        }

        private static int __GetInteger(System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory> context, string name)
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
