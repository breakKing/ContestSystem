import Ckeditor from "@ckeditor/ckeditor5-vue";
import ClassicEditor from "@ckeditor/ckeditor5-build-classic";

let CkeditorMixin = {
    components: {
        ckeditor: Ckeditor.component
    },
    data() {
        return {
            editor: ClassicEditor,
            editorConfig: {},
        }
    }
};
export {CkeditorMixin}