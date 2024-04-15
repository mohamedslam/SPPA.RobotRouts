using System;
using System.Collections;

#if STIDRAWING
using Image = Stimulsoft.Drawing.Image;
using Graphics = Stimulsoft.Drawing.Graphics;
#else
using System.Drawing;
#endif

namespace Stimulsoft.System.Windows.Forms
{
    public class ImageList
    {
        private ImageCollection imageCollection;

        internal class Indexer
        {
            private string key = String.Empty;
            private int index = -1;
            private bool useIntegerIndex = true;
            private ImageList imageList = null;

            public virtual ImageList ImageList
            {
                get { return imageList; }
                set { imageList = value; }
            }

            public virtual string Key
            {
                get { return key; }
                set
                {
                    index = -1;
                    key = (value == null ? String.Empty : value);
                    useIntegerIndex = false;
                }
            }

            public virtual int Index
            {
                get { return index; }
                set
                {
                    key = String.Empty;
                    index = value;
                    useIntegerIndex = true;
                }

            }

            public virtual int ActualIndex
            {
                get
                {
                    if (useIntegerIndex)
                    {
                        return Index;
                    }
                    else if (ImageList != null)
                    {
                        return ImageList.Images.IndexOfKey(Key);
                    }

                    return -1;
                }
            }
        }

        public ImageCollection Images
        {
            get
            {
                if (imageCollection == null)
                    imageCollection = new ImageCollection(this);
                return imageCollection;
            }
        }

        public global::System.Drawing.Size ImageSize { get; set; }

        public sealed class ImageCollection : IList
        {
            private ImageList owner;
            private ArrayList imageInfoCollection = new ArrayList();

            private int lastAccessedIndex = -1;

            /*public StringCollection Keys
            {
                get
                {
                    // pass back a copy of the current state.
                    StringCollection keysCollection = new StringCollection();

                    for (int i = 0; i < imageInfoCollection.Count; i++)
                    {
                        ImageInfo image = imageInfoCollection[i] as ImageInfo;
                        if ((image != null) && (image.Name != null) && (image.Name.Length != 0))
                        {
                            keysCollection.Add(image.Name);
                        }
                        else
                        {
                            keysCollection.Add(string.Empty);
                        }
                    }
                    return keysCollection;
                }
            }*/

            internal ImageCollection(ImageList owner)
            {
                this.owner = owner;
            }

            internal void ResetKeys()
            {
                if (imageInfoCollection != null)
                    imageInfoCollection.Clear();

                for (int i = 0; i < this.Count; i++)
                {
                    imageInfoCollection.Add(new ImageCollection.ImageInfo());
                }
            }

            private void AssertInvariant()
            {
            }

            public int Count
            {
                get
                {
                    /*if (owner.HandleCreated)
                    {
                        return SafeNativeMethods.ImageList_GetImageCount(new HandleRef(owner, owner.Handle));
                    }
                    else
                    {*/
                    int count = 0;
                    /*foreach (Original original in owner.originals)
                    {
                        if (original != null)
                        {
                            count += original.nImages;
                        }
                    }*/
                    return count;
                    /*}*/
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public bool Empty
            {
                get
                {
                    return Count == 0;
                }
            }

            public Image this[int index]
            {
                get
                {
                    /*if (index < 0 || index >= Count)
                        throw new ArgumentOutOfRangeException("index", SR.GetString(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));*/
                    //return owner.GetBitmap(index);
                    return null;
                }
                set
                {
                    /*if (index < 0 || index >= Count)
                        throw new ArgumentOutOfRangeException("index", SR.GetString(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));

                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }

                    if (!(value is Bitmap))
                        throw new ArgumentException(SR.GetString(SR.ImageListBitmap));

                    AssertInvariant();
                    Bitmap bitmap = (Bitmap)value;

                    bool ownsImage = false;
                    if (owner.UseTransparentColor)
                    {
                        // Since there's no ImageList_ReplaceMasked, we need to generate
                        // a transparent bitmap
                        Bitmap source = bitmap;
                        bitmap = (Bitmap)bitmap.Clone();
                        bitmap.MakeTransparent(owner.transparentColor);
                        ownsImage = true;
                    }

                    try
                    {
                        IntPtr hMask = ControlPaint.CreateHBitmapTransparencyMask(bitmap);
                        IntPtr hBitmap = ControlPaint.CreateHBitmapColorMask(bitmap, hMask);
                        bool ok = SafeNativeMethods.ImageList_Replace(new HandleRef(owner, owner.Handle), index, new HandleRef(null, hBitmap), new HandleRef(null, hMask));
                        SafeNativeMethods.DeleteObject(new HandleRef(null, hBitmap));
                        SafeNativeMethods.DeleteObject(new HandleRef(null, hMask));

                        if (!ok)
                            throw new InvalidOperationException(SR.GetString(SR.ImageListReplaceFailed));

                    }
                    finally
                    {
                        if (ownsImage)
                        {
                            bitmap.Dispose();
                        }
                    }*/
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    if (value is Image)
                    {
                        this[index] = (Image)value;
                    }
                    else
                    {
                        //throw new ArgumentException(SR.GetString(SR.ImageListBadImage), "value");
                    }
                }
            }

            public Image this[string key]
            {
                get
                {
                    // We do not support null and empty string as valid keys.
                    if ((key == null) || (key.Length == 0))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }

                }
            }

            public void Add(string key, Image image)
            {
                // Store off the name.
                ImageInfo imageInfo = new ImageInfo();
                imageInfo.Name = key;

                // Add the image to the IList
                //Original original = new Original(image, OriginalOptions.Default);
                //Add(original, imageInfo);

            }

            /*public void Add(string key, Icon icon)
            {
                // Store off the name.
                ImageInfo imageInfo = new ImageInfo();
                imageInfo.Name = key;

                // Add the image to the IList
                //Original original = new Original(icon, OriginalOptions.Default);
                Add(original, imageInfo);


            }*/


            int IList.Add(object value)
            {
                if (value is Image)
                {
                    Add((Image)value);
                    return Count - 1;
                }
                else
                {
                    throw new ArgumentException(/*SR.GetString(SR.ImageListBadImage), "value"*/);
                }
            }

            /*public void Add(Icon value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                //Add(new Original(value.Clone(), OriginalOptions.OwnsImage), null); // WHY WHY WHY do we clone here...
                // changing it now is a breaking change, so we have to keep track of this specific icon and dispose that
            }*/

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageList.ImageCollection.Add1"]/*' />
            /// <devdoc>
            ///     Add the given image to the ImageList.
            /// </devdoc>
            public void Add(Image value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                //Original original = new Original(value, OriginalOptions.Default);
                //Add(original, null);
            }

            public int Add(Image value, global::System.Drawing.Color transparentColor)
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                //Original original = new Original(value, OriginalOptions.CustomTransparentColor,
                //                                 transparentColor);
                //return Add(original, null);
                return 0;
            }

            /*private int Add(Original original, ImageInfo imageInfo)
            {
                if (original == null || original.image == null)
                {
                    throw new ArgumentNullException("original");
                }

                int index = -1;

                AssertInvariant();

                if (original.image is Bitmap)
                {
                    if (owner.originals != null)
                    {
                        index = owner.originals.Add(original);
                    }

                    if (owner.HandleCreated)
                    {
                        bool ownsBitmap = false;
                        Bitmap bitmapValue = owner.CreateBitmap(original, out ownsBitmap);
                        index = owner.AddToHandle(original, bitmapValue);
                        if (ownsBitmap)
                            bitmapValue.Dispose();
                    }
                }
                else if (original.image is Icon)
                {
                    if (owner.originals != null)
                    {
                        index = owner.originals.Add(original);
                    }
                    if (owner.HandleCreated)
                    {
                        index = owner.AddIconToHandle(original, (Icon)original.image);
                        // NOTE: if we own the icon (it's been created by us) this WILL dispose the icon to avoid a GDI leak
                        // **** original.image is NOT LONGER VALID AFTER THIS POINT ***
                    }
                }
                else
                {
                    throw new ArgumentException(SR.GetString(SR.ImageListBitmap));
                }

                // update the imageInfoCollection
                // support AddStrip
                if ((original.options & OriginalOptions.ImageStrip) != 0)
                {
                    for (int i = 0; i < original.nImages; i++)
                    {
                        imageInfoCollection.Add(new ImageInfo());
                    }
                }
                else
                {
                    if (imageInfo == null)
                        imageInfo = new ImageInfo();
                    imageInfoCollection.Add(imageInfo);
                }

                if (!owner.inAddRange)
                    owner.OnChangeHandle(new EventArgs());

                return index;
            }*/

            public void AddRange(Image[] images)
            {
                /*if (images == null)
                {
                    throw new ArgumentNullException("images");
                }
                owner.inAddRange = true;
                foreach (Image image in images)
                {
                    Add(image);
                }
                owner.inAddRange = false;
                owner.OnChangeHandle(new EventArgs());*/
            }

            /*public int AddStrip(Image value)
            {

                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                // strip width must be a positive multiple of image list width
                //
                if (value.Width == 0 || (value.Width % owner.ImageSize.Width) != 0)
                    throw new ArgumentException(SR.GetString(SR.ImageListStripBadWidth), "value");
                if (value.Height != owner.ImageSize.Height)
                    throw new ArgumentException(SR.GetString(SR.ImageListImageTooShort), "value");

                int nImages = value.Width / owner.ImageSize.Width;

                Original original = new Original(value, OriginalOptions.ImageStrip, nImages);

                return Add(original, null);
            }*/

            public void Clear()
            {
                AssertInvariant();
                /*if (owner.originals != null)
                    owner.originals.Clear();

                imageInfoCollection.Clear();

                if (owner.HandleCreated)
                    SafeNativeMethods.ImageList_Remove(new HandleRef(owner, owner.Handle), -1);

                owner.OnChangeHandle(new EventArgs());*/
            }

            public bool Contains(Image image)
            {
                throw new NotSupportedException();
            }

            bool IList.Contains(object image)
            {
                if (image is Image)
                {
                    return Contains((Image)image);
                }
                else
                {
                    return false;
                }
            }

            public bool ContainsKey(string key)
            {
                return IsValidIndex(IndexOfKey(key));
            }

            public int IndexOf(Image image)
            {
                throw new NotSupportedException();
            }

            int IList.IndexOf(object image)
            {
                if (image is Image)
                {
                    return IndexOf((Image)image);
                }
                else
                {
                    return -1;
                }
            }

            public int IndexOfKey(String key)
            {
                // Step 0 - Arg validation
                if ((key == null) || (key.Length == 0))
                {
                    return -1; // we dont support empty or null keys.
                }


                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    //if ((imageInfoCollection[lastAccessedIndex] != null) &&
                    //    (WindowsFormsUtils.SafeCompareStrings(((ImageInfo)imageInfoCollection[lastAccessedIndex]).Name, key, /* ignoreCase = */ true)))
                    //{
                    //    return lastAccessedIndex;
                    //}
                }

                // step 2 - search for the item
                for (int i = 0; i < this.Count; i++)
                {
                    //if ((imageInfoCollection[i] != null) &&
                    //        (WindowsFormsUtils.SafeCompareStrings(((ImageInfo)imageInfoCollection[i]).Name, key, /* ignoreCase = */ true)))
                    //{
                    //    lastAccessedIndex = i;
                    //    return i;
                    //}
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }


            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            private bool IsValidIndex(int index)
            {
                return ((index >= 0) && (index < this.Count));
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                AssertInvariant();
                for (int i = 0; i < Count; ++i)
                {
                    //dest.SetValue(owner.GetBitmap(i), index++);
                }
            }

            public IEnumerator GetEnumerator()
            {
                // Forces handle creation

                AssertInvariant();
                Image[] images = new Image[Count];
                /*for (int i = 0; i < images.Length; ++i)
                    images[i] = owner.GetBitmap(i);*/

                return images.GetEnumerator();
            }

            public void Remove(Image image)
            {
                throw new NotSupportedException();
            }

            /// <include file='doc\ImageList.uex' path='docs/doc[@for="ImageCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object image)
            {
                /*if (image is Image)
                {
                    Remove((Image)image);
                    owner.OnChangeHandle(new EventArgs());
                }*/
            }

            public void RemoveAt(int index)
            {
                /*if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException("index", SR.GetString(SR.InvalidArgument, "index", index.ToString(CultureInfo.CurrentCulture)));

                AssertInvariant();
                bool ok = SafeNativeMethods.ImageList_Remove(new HandleRef(owner, owner.Handle), index);
                if (!ok)
                {
                    throw new InvalidOperationException(SR.GetString(SR.ImageListRemoveFailed));
                }
                else
                {
                    if ((imageInfoCollection != null) && (index >= 0 && index < imageInfoCollection.Count))
                    {
                        imageInfoCollection.RemoveAt(index);
                        owner.OnChangeHandle(new EventArgs());
                    }
                }*/
            }

            public void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            public void SetKeyName(int index, string name)
            {
                if (!IsValidIndex(index))
                {
                    throw new IndexOutOfRangeException(); // 
                }

                if (imageInfoCollection[index] == null)
                {
                    imageInfoCollection[index] = new ImageInfo();
                }

                ((ImageInfo)imageInfoCollection[index]).Name = name;
            }

            internal class ImageInfo
            {
                private string name;
                public ImageInfo()
                {
                }

                public string Name
                {
                    get { return name; }
                    set { name = value; }
                }
            }

        }

        public void Draw(Graphics graphics, int x, int y, int width, int height, int imageIndex)
        {
            throw new NotImplementedException();
        }

        public ColorDepth ColorDepth { get; set; }
    }
}
