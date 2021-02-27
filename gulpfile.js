/// <binding BeforeBuild='sass' />
"use strict";

var gulp = require("gulp"),
    sass = require("gulp-sass");

var paths = {
    clientApp: "ClientApp/"
};

gulp.task("sass", function () {
    return gulp.src(paths.clientApp + "src/assets/main.scss")
        .pipe(sass())
        .pipe(gulp.dest(paths.clientApp + 'public/css'));
});