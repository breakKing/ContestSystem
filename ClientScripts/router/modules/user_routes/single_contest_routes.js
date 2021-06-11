import ContestPage from "../../../views/user/contests/single_contest/ContestPage";
import ContestInformationComponent from "../../../components/user/contests/ContestInformationComponent";
import TaskComponent from "../../../components/user/contests/participating/TaskComponent";

export default {
    path: 'contest',
    name: 'MainContestPage',
    component: ContestPage,
    meta: {
        authorize: ['user']
    },
    children: [
        {
            path: ':contest_id',
            name: 'ContestPage',
            component: ContestInformationComponent,
            props: true,
            meta: {
                authorize: ['user']
            },
        },
        {
            path: ':contest_id/participate/:task_id?',
            name: 'ContestParticipatingPage',
            component: TaskComponent,
            props: true,
            meta: {
                authorize: ['user']
            },
        },
    ]
}
