import "core-js/es7/reflect";
import "zone.js/dist/zone";
import "zone.js/dist/zone-patch-electron";

if (process.env.ENV === "production") {
    // Production
} else {
    // Development
    Error["stackTraceLimit"] = Infinity;
    require("zone.js/dist/long-stack-trace-zone");
}
