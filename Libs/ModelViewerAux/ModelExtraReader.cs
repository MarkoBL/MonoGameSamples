using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ModelViewerAux
{
    public class ModelExtraReader : ContentTypeReader<ModelExtra>
    {
        protected override ModelExtra Read(ContentReader input, ModelExtra existingInstance)
        {
            ModelExtra extra = new ModelExtra();
            extra.Skeleton = input.ReadObject<List<int>>();
            extra.Clips = input.ReadObject<List<AnimationClip>>();

            return extra;
        }
    }
}
