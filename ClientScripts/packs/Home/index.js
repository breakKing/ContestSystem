import {createApp} from 'vue'
import MainComponent from "../../components/MainComponent";
import store from "../../store/index";
import router from "../../router/index";
import axios from 'axios'
import VueAxios from 'vue-axios'
import $ from "jquery";
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import CKEditor from '@ckeditor/ckeditor5-vue';
import '../../styles/style.scss';
const app = createApp({
    components: {
        MainComponent
    },
    template: '<main-component></main-component>'
})
app.use(VueAxios, axios)
app.use(store)
app.use(router)
app.use( CKEditor )
app.mount('#app')