import {createApp} from 'vue'
import {Breadcrumb} from 'ant-design-vue';
import MainComponent from "../../components/MainComponent";
import store from "../../store/index";
import router from "../../router/index";
import axios from 'axios'
import VueAxios from 'vue-axios'
import $ from "jquery";
import 'bootstrap';
import 'bootstrap/scss/bootstrap.scss';
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'vue-prism-editor/dist/prismeditor.min.css';
import 'prismjs/themes/prism-tomorrow.css'; 
import '../../styles/style.scss';
import generateFingerprint from '../../services/fingerprint-loader';
import { configureApi, createApi } from '../../services/api-configurator';
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import ruLang from 'element-plus/es/locale/lang/ru'

generateFingerprint(store)

let apiAxios = createApi()
apiAxios = configureApi(apiAxios, store)

const app = createApp({
    components: {
        MainComponent
    },
    template: '<main-component></main-component>'
})
app.use(VueAxios, apiAxios)
app.use(store)
app.use(router)
app.use(Breadcrumb)
app.use(ElementPlus, {locale: ruLang})
app.mount('#app')