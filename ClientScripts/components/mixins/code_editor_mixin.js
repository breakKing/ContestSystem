import { PrismEditor } from 'vue-prism-editor';
import { highlight, languages } from "prismjs"
import 'prismjs/components/prism-c';

export default {
    components: {
        PrismEditor
    },
    methods: {
        highlighter(code) {
            return highlight(code, languages.c, 'c')
        }
    }
}