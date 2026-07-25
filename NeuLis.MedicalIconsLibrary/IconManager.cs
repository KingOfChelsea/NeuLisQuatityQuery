using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace NeuLis.MedicalIconsLibrary
{
    public class IconManager : IDisposable
    {
        private Dictionary<string, Image> _iconCache = new Dictionary<string, Image>();
        private string _basePath; // 路径

        // 单例模式确保全局唯一实例
        private static readonly Lazy<IconManager> _instance =
            new Lazy<IconManager>(() => new IconManager());

        public static IconManager Instance => _instance.Value;

        private IconManager()
        {
            // 自动定位到图标目录
            _basePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources", "Icons");

            // 自动扫描并加载所有图标文件到内存字典
            LoadAllIcons();
        }

        private void LoadAllIcons()
        {
            if (!Directory.Exists(_basePath)) return;
            // 递归搜索所有文件（包括子目录）
            var iconFiles = Directory.GetFiles(_basePath, "*.*", SearchOption.AllDirectories);

            foreach (var file in iconFiles)
            {
                var ext = Path.GetExtension(file).ToLower();
                if (ext == ".png" || ext == ".ico" || ext == ".jpg")
                {
                    var key = GetIconKey(file);  // 生成唯一路径
                    _iconCache[key] = Image.FromFile(file);  // 加载到内存
                }
            }
        }

        /// <summary>
        /// 根据相对路径生成唯一键
        /// </summary>
        private string GetIconKey(string fullPath)
        {
            var relativePath = fullPath.Replace(_basePath, "").TrimStart('\\');
            return relativePath.Replace("\\", ".").Replace(Path.GetExtension(fullPath), "");
        }

        /// <summary>
        /// 获取指定图标
        /// </summary>
        /// <param name="category">分类 (如 "Medical")</param>
        /// <param name="name">图标名称 (不含扩展名)</param>
        /// <returns></returns>
        public Image GetIcon(string category, string name)
        {
            var key = $"{category}.{name}";
            return _iconCache.ContainsKey(key) ? _iconCache[key] : null; // ContainsKey 判断是否存在  存在则返回对应的Image对象
        }

        /// <summary>
        /// 绑定图标到PictureBox
        /// </summary>
        public void BindToPictureBox(PictureBox pictureBox, string category, string name)
        {
            var icon = GetIcon(category, name);
            if (icon != null)
            {
                pictureBox.Image = icon;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        /// <summary>
        /// 绑定图标到Button
        /// </summary>
        public void BindToButton(Button button, string category, string name)
        {
            var icon = GetIcon(category, name);
            if (icon != null)
            {
                button.Image = icon;
                button.ImageAlign = ContentAlignment.MiddleLeft;
                button.TextImageRelation = TextImageRelation.ImageBeforeText;
            }
        }

        /// <summary>
        /// 填充ImageList控件
        /// </summary>
        public void PopulateImageList(ImageList imageList, params string[] iconKeys)
        {
            imageList.Images.Clear();
            imageList.ImageSize = new Size(32, 32); // 统一尺寸

            foreach (var key in iconKeys)
            {
                var parts = key.Split('.');
                if (parts.Length == 2)
                {
                    var icon = GetIcon(parts[0], parts[1]);
                    if (icon != null)
                    {
                        imageList.Images.Add(key, icon);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (var img in _iconCache.Values)
            {
                img?.Dispose();
            }
            _iconCache.Clear();
        }
    }
}
