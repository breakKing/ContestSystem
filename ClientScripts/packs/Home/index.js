import {createApp} from 'vue'
import {Breadcrumb} from 'ant-design-vue';
import MainComponent from "../../components/MainComponent";
import store from "../../store/index";
import router from "../../router/index";
import axios from 'axios'
import VueAxios from 'vue-axios'
import $ from "jquery";
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'vue-prism-editor/dist/prismeditor.min.css';
import 'prismjs/themes/prism-tomorrow.css'; 
import '../../styles/style.scss';
import generateFingerprint from '../../services/fingerprint-loader';
import { configureApi, createApi } from '../../services/api-configurator';

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
app.mount('#app')