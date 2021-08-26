import ModeratorNotModeratedRuleSetsPage from "../../../views/moder/rule_sets/ModeratorNotModeratedRuleSetsPage";
import ModeratorApprovedRuleSetsPage from "../../../views/moder/rule_sets/ModeratorApprovedRuleSetsPage";
import ModeratorRejectedRuleSetsPage from "../../../views/moder/rule_sets/ModeratorRejectedRuleSetsPage";
import ModeratorRuleSetModerationPage from "../../../views/moder/rule_sets/ModeratorRuleSetModerationPage";

export default [
    {
        path: 'rule-sets/not-moderated',
        name: 'ModeratorNotModeratedRuleSetsPage',
        component: ModeratorNotModeratedRuleSetsPage,
        props: true,
        meta: {
            authorize: ['moderator'],
            hide_sidebar: false
        },
    },
    {
        path: 'rule-sets/approved',
        name: 'ModeratorApprovedRuleSetsPage',
        component: ModeratorApprovedRuleSetsPage,
        props: true,
        meta: {
            authorize: ['moderator'],
            hide_sidebar: false
        },
    },
    {
        path: 'rule-sets/rejected',
        name: 'ModeratorRejectedRuleSetsPage',
        component: ModeratorRejectedRuleSetsPage,
        props: true,
        meta: {
            authorize: ['moderator'],
            hide_sidebar: false
        },
    },
    {
        path: 'rule-sets/:rule_set_id',
        name: 'ModeratorRuleSetModerationPage',
        component: ModeratorRuleSetModerationPage,
        props: true,
        meta: {
            authorize: ['moderator'],
            hide_sidebar: true
        },
    },
]
