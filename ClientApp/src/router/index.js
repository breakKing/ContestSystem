import { createWebHistory, createRouter } from "vue-router";
import Home from "@/components/Home.vue";
import TestDb from "@/components/TestDb.vue";
import ExamplesTest from "@/components/ExamplesTest.vue";

const routes = [
    {
        path: "/",
        name: "Home",
        component: Home
    },
    {
        path: "/testDb",
        name: "TestDb",
        component: TestDb
    }/*,
    {
        path: "/examplesTest",
        name: "ExamplesTest",
        component: ExamplesTest
    }*/
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;