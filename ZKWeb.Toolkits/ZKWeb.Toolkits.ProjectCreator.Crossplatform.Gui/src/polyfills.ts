import 'core-js/es7/reflect';

import 'zone.js/dist/zone';
// The following import fixes zone issues when Electron callbacks are used eg. Menu's.
import 'zone.js/dist/zone-patch-electron';

if (process.env.ENV === 'production') {
    // Production
} else {
    // Development
    Error['stackTraceLimit'] = Infinity;
    require('zone.js/dist/long-stack-trace-zone');
}
