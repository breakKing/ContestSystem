import ManagerPage from "../../views/moder/ManagerPage";

export default {
    path: '/manager',
    name: 'ManagerHome',
    component: ManagerPage,
    meta: {
        authorize: ['manager']
    }
}