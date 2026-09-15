namespace Markwardt.GodotServices;

/// <summary>
/// The paths of commonly-used <see cref="Godot.ProjectSettings"/> entries, sorted into one nested
/// static class per top-level settings category. Every key here is automatically registered as a
/// keyed <see cref="IMutableSetting{T}"/> singleton by <see cref="ServiceContainer"/> - inject it directly
/// with <see cref="InjectAttribute"/> or resolve it as a keyed service, no manual registration needed.
/// Each path doubles as its own registration key, since it's already a stable, globally unique
/// string - there's no need for a separate, project-invented key the way a consuming project's
/// own settings need one. Declared as <see langword="const"/> fields, rather than this project's
/// usual static-property convention for fixed values, since <see cref="InjectAttribute"/> is only
/// able to accept compile-time constants as constructor arguments.
/// </summary>
public static class ProjectSettings
{
    /// <summary>
    /// Settings under <c>application/</c>: identity, startup, and boot-screen configuration.
    /// </summary>
    public static class Application
    {
        /// <summary>The project's display name, an <see cref="IMutableSetting{T}"/> of <see langword="string"/>.</summary>
        public const string ConfigName = "application/config/name";

        /// <summary>The project's description, an <see cref="IMutableSetting{T}"/> of <see langword="string"/>.</summary>
        public const string ConfigDescription = "application/config/description";

        /// <summary>The <c>res://</c> path to the project's icon, an <see cref="IMutableSetting{T}"/> of <see langword="string"/>.</summary>
        public const string ConfigIcon = "application/config/icon";

        /// <summary>The <c>res://</c> path to the scene that runs on startup, an <see cref="IMutableSetting{T}"/> of <see langword="string"/>.</summary>
        public const string RunMainScene = "application/run/main_scene";

        /// <summary>The boot splash background color, an <see cref="IMutableSetting{T}"/> of <see cref="Color"/>.</summary>
        public const string BootSplashBgColor = "application/boot_splash/bg_color";

        /// <summary>The <c>res://</c> path to the boot splash image, an <see cref="IMutableSetting{T}"/> of <see langword="string"/>.</summary>
        public const string BootSplashImage = "application/boot_splash/image";
    }

    /// <summary>
    /// Settings under <c>display/window/</c>: the main window's default size and behavior. Prefer
    /// <see cref="GameSettings"/> for the ones that need to change live, at runtime - these
    /// project settings are its defaults, read once at startup.
    /// </summary>
    public static class Display
    {
        /// <summary>The window's default width, in pixels, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string WindowWidth = "display/window/size/viewport_width";

        /// <summary>The window's default height, in pixels, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string WindowHeight = "display/window/size/viewport_height";

        /// <summary>The window's default display mode, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string WindowMode = "display/window/size/mode";

        /// <summary>Whether the window is resizable by the player, an <see cref="IMutableSetting{T}"/> of <see langword="bool"/>.</summary>
        public const string WindowResizable = "display/window/size/resizable";

        /// <summary>The viewport stretch mode, an <see cref="IMutableSetting{T}"/> of <see langword="string"/>.</summary>
        public const string StretchMode = "display/window/stretch/mode";

        /// <summary>The viewport stretch aspect, an <see cref="IMutableSetting{T}"/> of <see langword="string"/>.</summary>
        public const string StretchAspect = "display/window/stretch/aspect";

        /// <summary>The window's default vertical sync mode, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string VSyncMode = "display/window/vsync/vsync_mode";
    }

    /// <summary>
    /// Settings under <c>audio/</c>: bus layout and general audio behavior.
    /// </summary>
    public static class Audio
    {
        /// <summary>The <c>res://</c> path to the default audio bus layout resource, an <see cref="IMutableSetting{T}"/> of <see langword="string"/>.</summary>
        public const string DefaultBusLayout = "audio/buses/default_bus_layout";

        /// <summary>The default audio playback type, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string DefaultPlaybackType = "audio/general/default_playback_type";
    }

    /// <summary>
    /// Settings under <c>rendering/</c>: anti-aliasing, shadows, and default environment values.
    /// </summary>
    public static class Rendering
    {
        /// <summary>The 2D MSAA quality, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string Msaa2D = "rendering/anti_aliasing/quality/msaa_2d";

        /// <summary>The 3D MSAA quality, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string Msaa3D = "rendering/anti_aliasing/quality/msaa_3d";

        /// <summary>The screen-space anti-aliasing mode, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string ScreenSpaceAA = "rendering/anti_aliasing/quality/screen_space_aa";

        /// <summary>The default environment clear color, an <see cref="IMutableSetting{T}"/> of <see cref="Color"/>.</summary>
        public const string DefaultClearColor = "rendering/environment/defaults/default_clear_color";

        /// <summary>The 3D resolution scaling mode, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string Scaling3DMode = "rendering/scaling_3d/mode";

        /// <summary>The 3D resolution scale factor, an <see cref="IMutableSetting{T}"/> of <see langword="float"/>.</summary>
        public const string Scaling3DScale = "rendering/scaling_3d/scale";

        /// <summary>The directional shadow map size, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string DirectionalShadowSize = "rendering/lights_and_shadows/directional_shadow/size";

        /// <summary>Whether occlusion culling is enabled, an <see cref="IMutableSetting{T}"/> of <see langword="bool"/>.</summary>
        public const string UseOcclusionCulling = "rendering/occlusion_culling/use_occlusion_culling";
    }

    /// <summary>
    /// Settings under <c>physics/</c>: default gravity and simulation tick rate.
    /// </summary>
    public static class Physics
    {
        /// <summary>The default 2D gravity magnitude, an <see cref="IMutableSetting{T}"/> of <see langword="float"/>.</summary>
        public const string Gravity2D = "physics/2d/default_gravity";

        /// <summary>The default 2D gravity direction, an <see cref="IMutableSetting{T}"/> of <see cref="Vector2"/>.</summary>
        public const string GravityVector2D = "physics/2d/default_gravity_vector";

        /// <summary>The default 3D gravity magnitude, an <see cref="IMutableSetting{T}"/> of <see langword="float"/>.</summary>
        public const string Gravity3D = "physics/3d/default_gravity";

        /// <summary>The default 3D gravity direction, an <see cref="IMutableSetting{T}"/> of <see cref="Vector3"/>.</summary>
        public const string GravityVector3D = "physics/3d/default_gravity_vector";

        /// <summary>The number of physics ticks per second, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string PhysicsTicksPerSecond = "physics/common/physics_ticks_per_second";

        /// <summary>The maximum physics steps to process per rendered frame, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string MaxPhysicsStepsPerFrame = "physics/common/max_physics_steps_per_frame";
    }

    /// <summary>
    /// Settings under <c>input_devices/</c>: touch/mouse input emulation.
    /// </summary>
    public static class InputDevices
    {
        /// <summary>Whether mouse input is emulated as touch input, an <see cref="IMutableSetting{T}"/> of <see langword="bool"/>.</summary>
        public const string EmulateTouchFromMouse = "input_devices/pointing/emulate_touch_from_mouse";

        /// <summary>Whether touch input is emulated as mouse input, an <see cref="IMutableSetting{T}"/> of <see langword="bool"/>.</summary>
        public const string EmulateMouseFromTouch = "input_devices/pointing/emulate_mouse_from_touch";
    }

    /// <summary>
    /// Settings under <c>debug/</c>: development-time diagnostics.
    /// </summary>
    public static class Debug
    {
        /// <summary>An artificial engine-wide FPS cap for testing, an <see cref="IMutableSetting{T}"/> of <see langword="int"/>.</summary>
        public const string ForceFps = "debug/settings/fps/force_fps";

        /// <summary>Whether the current FPS is printed to standard output, an <see cref="IMutableSetting{T}"/> of <see langword="bool"/>.</summary>
        public const string PrintFps = "debug/settings/stdout/print_fps";
    }

    /// <summary>
    /// Settings under <c>gui/</c>: general user-interface behavior.
    /// </summary>
    public static class Gui
    {
        /// <summary>Whether controls snap to pixel boundaries, an <see cref="IMutableSetting{T}"/> of <see langword="bool"/>.</summary>
        public const string SnapControlsToPixels = "gui/common/snap_controls_to_pixels";
    }
}
