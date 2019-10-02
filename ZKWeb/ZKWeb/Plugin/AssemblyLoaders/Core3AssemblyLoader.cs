#if NETCORE_3
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using ZKWebStandard.Extensions;

namespace ZKWeb.Plugin.AssemblyLoaders
{
    /// <summary>
    /// Assembly loader for .Net Core 3.0 and later, supports unloading<br/>
    /// .Net Core 3.0 以上使用的程序集加载器，支持卸载<br/>
    /// </summary>
    internal class Core3AssemblyLoader : AssemblyLoaderBase
    {
        /// <summary>
        /// The load context<br/>
        /// 加载上下文<br/>
        /// </summary>
        private AssemblyLoadContext Context { get; set; }
        /// <summary>
        /// Possible assembly name suffixes<br/>
        /// Use to load assemblies by short name<br/>
        /// 预置的程序集名称后续<br/>
        /// 用于支持根据短名称加载程序集<br/>
        /// </summary>
        private IList<string> PossibleAssemblyNameSuffixes { get; set; }

        /// <summary>
        /// Initialize<br/>
        /// 初始化<br/>
        /// </summary>
        public Core3AssemblyLoader()
        {
            // AssemblyLoadContext is a abstract class in netstandard2.1, and Unload method only available on netcoreapp3.0
            // netstandard really become a joke, like all jokes microsoft made :|
            Context = new AssemblyLoadContext(
                name: "ZKWeb-" + DateTime.Now.ToString(),
                isCollectible: true);
            Context.Resolving += AssemblyResolver;
            PossibleAssemblyNameSuffixes = new List<string>() {
                ", Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                ", Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            };
            // copy assembly from default context
            foreach (var assembly in AssemblyLoadContext.Default.Assemblies)
            {
                var path = assembly.Location;
                if (File.Exists(path))
                {
                    Context.LoadFromAssemblyPath(path);
                }
            }
        }

        /// <summary>
        /// Load assembly by name<br/>
        /// 根据名称加载程序集<br/> 
        /// </summary>
        public override Assembly Load(string name)
        {
            // Replace name if replacement exists
            name = ReplacementAssemblies.GetOrDefault(name, name);
            Assembly assembly = null;
            try
            {
                // Try load directly
                assembly = Load(new AssemblyName(name));
            }
            catch
            {
                // If load failed, add suffixes and try again
                foreach (var suffix in PossibleAssemblyNameSuffixes)
                {
                    try
                    {
                        assembly = Load(new AssemblyName(name + suffix));
                        break;
                    }
                    catch
                    {
                        // Retry next round
                    }
                }
                if (assembly == null)
                {
                    throw;
                }
            }
            return HandleLoadedAssembly(assembly);
        }

        /// <summary>
        /// Load assembly by name object<br/>
        /// 根据名称对象加载程序集<br/>
        /// </summary>
        public override Assembly Load(AssemblyName assemblyName)
        {
            var assembly = Context.LoadFromAssemblyName(assemblyName);
            return HandleLoadedAssembly(assembly);
        }

        /// <summary>
        /// Load assembly from it's binary contents<br/>
        /// 根据二进制内容加载程序集<br/>
        /// </summary>
        public override Assembly Load(byte[] rawAssembly)
        {
            using (var stream = new MemoryStream(rawAssembly))
            {
                var assembly = Context.LoadFromStream(stream);
                return HandleLoadedAssembly(assembly);
            }
        }

        /// <summary>
        /// Load assembly from file path<br/>
        /// 根据文件路径加载程序集<br/>
        /// </summary>
        public override Assembly LoadFile(string path)
        {
            var assembly = Context.LoadFromAssemblyPath(path);
            return HandleLoadedAssembly(assembly);
        }

        /// <summary>
        /// Return whether this loader is unloadable.<br/>
        /// 返回此加载器是否支持卸载<br/>
        /// </summary>
        public override bool Unloadable { get { return true; } }

        /// <summary>
        /// Unload assemblies from this loader, only supported on some platforms<br/>
        /// 卸载从此加载器加载的程序集，只支持部分平台<br/>
        /// </summary>
        public override void Unload()
        {
            Context.Unload();
        }

        /// <summary>
        /// Assembly resolve event handler<br/>
        /// 程序集解决事件的处理器<br/>
        /// </summary>
        /// <returns></returns>
        private Assembly AssemblyResolver(AssemblyLoadContext context, AssemblyName name)
        {
            // If assembly already loaded, return the loaded instance
            foreach (var assembly in GetLoadedAssemblies())
            {
                if (assembly.GetName().Name == name.Name)
                {
                    return assembly;
                }
            }
            // Try to load Assembly from plugin's reference directory
            var pluginManager = Application.Ioc.Resolve<PluginManager>();
            foreach (var plugin in pluginManager.Plugins)
            {
                var path = plugin.ReferenceAssemblyPath(name.Name);
                if (path != null)
                {
                    return context.LoadFromAssemblyPath(path);
                }
            }
            // It's not found, return null
            return null;
        }
    }
}
#endif
