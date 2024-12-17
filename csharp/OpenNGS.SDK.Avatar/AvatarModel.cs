using System;
using System.Collections.Generic;
using System.Text;

namespace OpenNGS.SDK.Avatar.Models
{
    /// <summary>
    /// 应用得到的形象视图模型
    /// </summary>
    public class AppPlayerVo
    {
        /// <summary>
        /// 形象名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 形象简介
        /// </summary>
        public string? Dec { get; set; }

        /// <summary>
        /// 用户形象个性标签
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 用户3D形象
        /// </summary>
        public AppAvatarVo? Avatar { get; set; }

        /// <summary>
        /// 用户的EEid
        /// </summary>
        public string? Eeid { get; set; }

    }

    public class AppAvatarVo
    {
        /// <summary>
        /// 索引Id
        /// </summary>
        public string? AvatarId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 角色介绍
        /// </summary>
        public string? Dec { get; set; }

        /// <summary>
        /// 所属分类
        /// </summary>
        public AppClassifVo? Classification { get; set; }

        /// <summary>
        /// 自定义标签
        /// </summary>
        public List<string>? Tags { get; set; }

        /// <summary> 
        /// 作者
        /// </summary>
        public AppUserMsgVo? Author { get; set; }

        /// <summary>
        /// 角色的资产
        /// </summary>
        public AppAssetsVo? Assets { get; set; }

    }

    public class AppClassifVo
    {

        /// <summary>
        /// 分类名
        /// </summary>
        public string Lable { get; set; }

    }

    public class AppUserMsgVo
    {

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// 用户的EEid
        /// </summary>
        public string? Eeid { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string? UserAvatar { get; set; }

    }

    public class AppAssetsVo
    {

        /// <summary>
        /// 图标
        /// </summary>
        public AppFileItemVo? Icon { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public AppFileItemVo? Img { get; set; }

        /// <summary>
        /// 模型
        /// </summary>
        public AppModVo? Mods { get; set; }
    }

    public class AppModVo
    {

        /// <summary>
        /// 模型面数
        /// </summary>
        public int? Face { get; set; }

        /// <summary>
        /// VRM类型
        /// </summary>
        public AppFileItemVo? Vrm { get; set; }

    }

    public class AppFileItemVo
    {

        /// <summary>
        /// 文件名
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// 文件对应的COS链接
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 文件键
        /// </summary>
        public string? FileKey { get; set; }

    }
}
