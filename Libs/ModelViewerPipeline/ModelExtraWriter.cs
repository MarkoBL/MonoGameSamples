using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using ModelViewerAux;

namespace ModelViewerPipeline
{
    [ContentTypeWriter]
    public class ModelExtraContentWriter : ContentTypeWriter<ModelExtraContent>
    {
        protected override void Write(ContentWriter output, ModelExtraContent extra)
        {
            output.WriteObject(extra.Skeleton);
            output.WriteObject(extra.Clips);
        }

        /// <summary>
        /// Tells the content pipeline what CLR type the custom
        /// model data will be loaded into at runtime.
        /// </summary>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "ModelViewerAux.ModelExtra, " +
                   "ModelViewerAux, Version=1.0.0.0, Culture=neutral";
        }

        /// <summary>
        /// Tells the content pipeline what worker type
        /// will be used to load the custom model data.
        /// </summary>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ModelExtraReader).AssemblyQualifiedName;
        }
    }
}
